using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace SharpFuzz
{
	/// <summary>
	/// American fuzzy lop instrumentation and fork server for .NET libraries.
	/// </summary>
	public static class Fuzzer
	{
		[DllImport("libc", SetLastError = true)]
		private static extern SharedMemoryHandle shmat(int shmid, IntPtr shmaddr, int shmflg);

		[DllImport("libc", SetLastError = true)]
		private static extern int shmdt(IntPtr shmaddr);

		/// <summary>
		/// Instrument method performs the afl-fuzz instrumentation
		/// of the <paramref name="source"/> assembly and places the
		/// instrumented assembly in <paramref name="destination"/>.
		/// </summary>
		/// <param name="source">The assembly to instrument.</param>
		/// <param name="destination">The destination path of the instrumented assembly.</param>
		public static void Instrument(string source, string destination)
		{
			ThrowIfNull(source, nameof(source));
			ThrowIfNull(destination, nameof(destination));

			var common = typeof(SharpFuzz.Common.Trace).Assembly.Location;
			var sourceModule = ModuleDefinition.ReadModule(source);
			var commonModule = ModuleDefinition.ReadModule(common);

			var traceType = commonModule.Types.Single(t => t.FullName == typeof(Common.Trace).FullName);
			var sharedMemDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.SharedMem));
			var prevLocationDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.PrevLocation));

			var sharedMemRef = sourceModule.ImportReference(sharedMemDef);
			var prevLocationRef = sourceModule.ImportReference(prevLocationDef);

			foreach (var type in sourceModule.Types)
			{
				foreach (var method in type.Methods)
				{
					if (method.HasBody)
					{
						Method.Instrument(sharedMemRef, prevLocationRef, method);
					}
				}
			}

			sourceModule.Write(destination);
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
		public static void Run(Action action)
		{
			ThrowIfNull(action, nameof(action));
			var s = Environment.GetEnvironmentVariable("__AFL_SHM_ID");

			if (s is null || !Int32.TryParse(s, out var shmid))
			{
				throw new Exception("This program can only be run under afl-fuzz.");
			}

			using (var shmaddr = shmat(shmid, IntPtr.Zero, 0))
			using (var r = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, "198")))
			using (var w = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, "199")))
			{
				var local = SharpFuzz.Common.Trace.SharedMem.AsSpan();
				var shared = shmaddr.Span(local.Length);

				shared[0] = 1;
				w.Write(0);

				var pid = Process.GetCurrentProcess().Id;

				while (true)
				{
					r.ReadInt32();
					w.Write(pid);

					local.Clear();
					Fault fault = Fault.None;

					try
					{
						action();
					}
					catch
					{
						fault = Fault.Crash;
					}

					local.CopyTo(shared);
					w.Write((int)fault);
				}
			}
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
