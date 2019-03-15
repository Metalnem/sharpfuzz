using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace SharpFuzz
{
	/// <summary>
	/// American fuzzy lop instrumentation and fork server for .NET libraries.
	/// </summary>
	public static partial class Fuzzer
	{
		private const int MapSize = 1 << 16;

		/// <summary>
		/// Instrument method performs the in-place afl-fuzz
		/// instrumentation of the <paramref name="source"/> assembly.
		/// </summary>
		/// <param name="source">The assembly to instrument.</param>
		/// <param name="matcher">
		/// A function that accepts the full name of the class and returns
		/// true if the class should be instrumented, false otherwise.
		/// </param>
		/// <returns>An ordered collection of instrumented types.</returns>
		public static IEnumerable<string> Instrument(string source, Func<string, bool> matcher)
		{
			ThrowIfNull(source, nameof(source));
			ThrowIfNull(matcher, nameof(matcher));

			SortedSet<string> types;

			using (var dst = new MemoryStream())
			{
				using (var src = ModuleDefMD.Load(source))
				{
					if (!src.IsILOnly)
					{
						throw new InstrumentationException("Cannot instrument mixed-mode assemblies.");
					}

					if (src.TypeExistsNormal(typeof(Common.Trace).FullName))
					{
						throw new InstrumentationException("The specified assembly is already instrumented.");
					}

					var common = typeof(Common.Trace).Assembly;
					var commonName = common.GetName().Name;

					if (src.GetAssemblyRefs().Any(name => name.Name == commonName))
					{
						throw new InstrumentationException("The specified assembly is already instrumented.");
					}

					if (Path.GetFileNameWithoutExtension(source) == "System.Private.CoreLib")
					{
						var traceType = GenerateTraceType(src);
						src.Types.Add(traceType);
						types = Instrument(src, dst, matcher, traceType);
					}
					else
					{
						using (var commonMod = ModuleDefMD.Load(common.Location))
						{
							var traceType = commonMod.Types.Single(t => t.FullName == typeof(Common.Trace).FullName);
							types = Instrument(src, dst, matcher, traceType);
						}
					}
				}

				dst.Position = 0;

				using (var file = File.Create(source))
				{
					dst.CopyTo(file);
				}
			}

			return types;
		}

		private static SortedSet<string> Instrument(ModuleDefMD src, Stream dst, Func<string, bool> matcher, TypeDef traceType)
		{
			var sharedMemDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.SharedMem));
			var prevLocationDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.PrevLocation));

			var sharedMemRef = src.Import(sharedMemDef);
			var prevLocationRef = src.Import(prevLocationDef);

			var types = new SortedSet<string>();

			foreach (var type in src.GetTypes())
			{
				if (type.HasMethods && matcher(type.FullName))
				{
					bool instrumented = false;

					foreach (var method in type.Methods)
					{
						if (method.HasBody)
						{
							Method.Instrument(sharedMemRef, prevLocationRef, method);

							if (!instrumented)
							{
								types.Add(type.FullName);
								instrumented = true;
							}
						}
					}
				}
			}

			src.Write(dst);
			return types;
		}

		private static TypeDefUser GenerateTraceType(ModuleDefMD mod)
		{
			var traceType = new TypeDefUser(
				typeof(SharpFuzz.Common.Trace).FullName,
				mod.CorLibTypes.Object.TypeDefOrRef
			);

			traceType.Attributes = TypeAttributes.Public
				| TypeAttributes.Abstract
				| TypeAttributes.Sealed
				| TypeAttributes.BeforeFieldInit;

			var sharedMemField = new FieldDefUser(
				nameof(Common.Trace.SharedMem),
				new FieldSig(new PtrSig(mod.CorLibTypes.Byte)),
				FieldAttributes.Public | FieldAttributes.Static
			);

			var prevLocationField = new FieldDefUser(
				nameof(Common.Trace.PrevLocation),
				new FieldSig(mod.CorLibTypes.Int32),
				FieldAttributes.Public | FieldAttributes.Static
			);

			traceType.Fields.Add(sharedMemField);
			traceType.Fields.Add(prevLocationField);

			var cctorSig = MethodSig.CreateStatic(mod.CorLibTypes.Void);
			var cctorImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;

			var cctorFlags = MethodAttributes.Private
				| MethodAttributes.HideBySig
				| MethodAttributes.SpecialName
				| MethodAttributes.RTSpecialName
				| MethodAttributes.Static;

			var cctor = new MethodDefUser(".cctor", cctorSig, cctorImplFlags, cctorFlags);
			traceType.Methods.Add(cctor);

			var body = new CilBody { InitLocals = false, MaxStack = 1 };
			cctor.Body = body;

			var local = new Local(mod.CorLibTypes.IntPtr);
			body.Variables.Add(local);

			var marshalType = mod.Types.Single(type => type.FullName == typeof(Marshal).FullName);
			var intPtrType = mod.Types.Single(type => type.FullName == typeof(IntPtr).FullName);

			var allocHGlobal = marshalType.FindMethod(
				nameof(Marshal.AllocHGlobal),
				MethodSig.CreateStatic(mod.CorLibTypes.IntPtr,
				mod.CorLibTypes.Int32)
			);

			var toPointer = intPtrType.FindMethod(
				nameof(IntPtr.ToPointer),
				MethodSig.CreateInstance(new PtrSig(mod.CorLibTypes.Void))
			);

			body.Instructions.Add(OpCodes.Ldc_I4.ToInstruction(MapSize));
			body.Instructions.Add(OpCodes.Call.ToInstruction(allocHGlobal));
			body.Instructions.Add(OpCodes.Stloc_0.ToInstruction());
			body.Instructions.Add(OpCodes.Ldloca_S.ToInstruction(local));
			body.Instructions.Add(OpCodes.Call.ToInstruction(toPointer));
			body.Instructions.Add(OpCodes.Stsfld.ToInstruction(sharedMemField));
			body.Instructions.Add(OpCodes.Ret.ToInstruction());

			return traceType;
		}

		/// <summary>
		/// Run method starts the .NET equivalent of AFL fork server.
		/// It repeatedly executes the passed action and reports the
		/// execution result to afl-fuzz. This function will only work
		/// if the executable that is calling it is running under afl-fuzz.
		/// </summary>
		/// <param name="action">
		/// Some action that calls the instrumented library. If an
		/// uncaught exception escapes the call, FAULT_CRASH execution
		/// status code is reported to afl-fuzz.
		/// </param>
		public static unsafe void Run(Action action)
		{
			ThrowIfNull(action, nameof(action));
			var s = Environment.GetEnvironmentVariable("__AFL_SHM_ID");

			if (s is null || !Int32.TryParse(s, out var shmid))
			{
				throw new Exception("This program can only be run under afl-fuzz.");
			}

			using (var shmaddr = Native.shmat(shmid, IntPtr.Zero, 0))
			using (var r = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, "198")))
			using (var w = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, "199")))
			{
				byte* sharedMem = (byte*)shmaddr.DangerousGetHandle();
				InitializeSharedMemory(sharedMem);

				w.Write(0);
				Setup(action, sharedMem);
				var pid = Process.GetCurrentProcess().Id;

				while (true)
				{
					r.ReadInt32();
					w.Write(pid);
					w.Write(Execute(action));
				}
			}
		}

		/// <summary>
		/// RunOnce method executes the passed action once and writes the
		/// trace bits to the shared memory. This function will only work
		/// if the executable that is calling it is running under afl-fuzz.
		/// </summary>
		/// <param name="action">
		/// Some action that calls the instrumented library.
		/// </param>
		public static unsafe void RunOnce(Action action)
		{
			ThrowIfNull(action, nameof(action));
			var s = Environment.GetEnvironmentVariable("__AFL_SHM_ID");

			if (s is null || !Int32.TryParse(s, out var shmid))
			{
				throw new Exception("This program can only be run under afl-fuzz.");
			}

			using (var shmaddr = Native.shmat(shmid, IntPtr.Zero, 0))
			{
				InitializeSharedMemory((byte*)shmaddr.DangerousGetHandle());
				action();
			}
		}

		private static unsafe void InitializeSharedMemory(byte* sharedMem)
		{
			Common.Trace.SharedMem = sharedMem;

			var types = typeof(object).Assembly.GetTypes();
			var traceType = types.FirstOrDefault(t => t.FullName == typeof(Common.Trace).FullName);

			if (traceType != null)
			{
				var sharedMemField = traceType.GetField(nameof(Common.Trace.SharedMem));
				sharedMemField.SetValue(null, System.Reflection.Pointer.Box(sharedMem, typeof(byte*)));
			}
		}

		// Initial run will usually have the different trace bits
		// from the subsequent runs because static constructors
		// and other types of static initialization are going to
		// be executed only once. To prevent "WARNING: Instrumentation
		// output varies across runs" message in the afl-fuzz, we can
		// safely ignore the first execution during the dry run.
		private static unsafe void Setup(Action action, byte* sharedMem)
		{
			Execute(action);
			new Span<byte>(sharedMem, MapSize).Clear();
		}

		private static int Execute(Action action)
		{
			try
			{
				action();
			}
			catch
			{
				return Fault.Crash;
			}

			return Fault.None;
		}

		private static void ThrowIfNull(object value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
		}
	}
}
