using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using dnlib.DotNet;

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

					foreach (var type in sourceMod.Types)
					{
						foreach (var method in type.Methods)
						{
							if (method.HasBody)
							{
								Method.Instrument(sharedMemRef, prevLocationRef, method);
							}
						}
					}

					sourceMod.Write(memory);
				}

				memory.Position = 0;

				using (var file = File.Create(source))
				{
					memory.CopyTo(file);
				}
			}
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
				Common.Trace.SharedMem = (byte*)shmaddr.DangerousGetHandle();
				w.Write(0);

				Setup(action);
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
		private static unsafe void Setup(Action action)
		{
			Execute(action);

			for (int i = 0; i < 65536; ++i)
			{
				Common.Trace.SharedMem[i] = 0;
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
