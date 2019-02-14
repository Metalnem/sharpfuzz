using System;
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
		/// <summary>
		/// Instrument method performs the in-place afl-fuzz
		/// instrumentation of the <paramref name="source"/> assembly.
		/// </summary>
		/// <param name="source">The assembly to instrument.</param>
		public static void Instrument(string source)
		{
			ThrowIfNull(source, nameof(source));

			using (var memory = new MemoryStream())
			{
				if (Path.GetFileNameWithoutExtension(source) == "System.Private.CoreLib")
				{
					InstrumentCoreLib(source, memory);
				}
				else
				{
					Instrument(source, memory);
				}

				memory.Position = 0;

				using (var file = File.Create(source))
				{
					memory.CopyTo(file);
				}
			}
		}

		private static void InstrumentCoreLib(string source, Stream destination)
		{
			using (var sourceMod = ModuleDefMD.Load(source))
			{
				if (sourceMod.TypeExistsNormal(typeof(Common.Trace).FullName))
				{
					throw new InstrumentationException("The specified assembly is already instrumented.");
				}

				var traceType = GenerateTraceType(sourceMod);
				sourceMod.Types.Add(traceType);

				var sharedMemDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.SharedMem));
				var prevLocationDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.PrevLocation));

				var sharedMemRef = sourceMod.Import(sharedMemDef);
				var prevLocationRef = sourceMod.Import(prevLocationDef);

				foreach (var type in sourceMod.GetTypes())
				{
					if (type.Namespace == "System.Globalization")
					{
						foreach (var method in type.Methods)
						{
							if (method.HasBody)
							{
								Method.Instrument(sharedMemRef, prevLocationRef, method);
							}
						}
					}
				}

				sourceMod.Write(destination);
			}
		}

		private static void Instrument(string source, Stream destination)
		{
			var common = typeof(Common.Trace).Assembly;
			var commonLoc = common.Location;
			var commonName = common.GetName().Name;

			using (var commonMod = ModuleDefMD.Load(commonLoc))
			using (var sourceMod = ModuleDefMD.Load(source))
			{
				if (sourceMod.GetAssemblyRefs().Any(name => name.Name == commonName))
				{
					throw new InstrumentationException("The specified assembly is already instrumented.");
				}

				var traceType = commonMod.Types.Single(t => t.FullName == typeof(Common.Trace).FullName);
				var sharedMemDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.SharedMem));
				var prevLocationDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.PrevLocation));

				var sharedMemRef = sourceMod.Import(sharedMemDef);
				var prevLocationRef = sourceMod.Import(prevLocationDef);

				foreach (var type in sourceMod.GetTypes())
				{
					foreach (var method in type.Methods)
					{
						if (method.HasBody)
						{
							Method.Instrument(sharedMemRef, prevLocationRef, method);
						}
					}
				}

				sourceMod.Write(destination);
			}
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

			body.Instructions.Add(OpCodes.Ldc_I4.ToInstruction(65536));
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
				Common.Trace.SharedMem = sharedMem;

				var types = typeof(object).Assembly.GetTypes();
				var traceType = types.FirstOrDefault(t => t.FullName == typeof(Common.Trace).FullName);

				if (traceType != null)
				{
					var sharedMemField = traceType.GetField(nameof(Common.Trace.SharedMem));
					sharedMemField.SetValue(null, System.Reflection.Pointer.Box(sharedMem, typeof(byte*)));
				}

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
				Common.Trace.SharedMem = (byte*)shmaddr.DangerousGetHandle();
				action();
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

			for (int i = 0; i < 65536; ++i)
			{
				sharedMem[i] = 0;
			}
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
