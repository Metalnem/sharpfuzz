using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace SharpFuzz
{
	public static class Fuzzer
	{
		[DllImport("libc", SetLastError = true)]
		private static extern SharedMemoryHandle shmat(int shmid, IntPtr shmaddr, int shmflg);

		[DllImport("libc", SetLastError = true)]
		private static extern int shmdt(IntPtr shmaddr);

		public static void Instrument(string source, string destination)
		{
			var common = typeof(SharpFuzz.Common.Trace).Assembly.Location;
			var sourceModule = ModuleDefinition.ReadModule(source);
			var commonModule = ModuleDefinition.ReadModule(common);

			var traceType = commonModule.Types.Single(t => t.FullName == "SharpFuzz.Common.Trace");
			var sharedMemDef = traceType.Fields.Single(f => f.Name == "SharedMem");
			var prevLocationDef = traceType.Fields.Single(f => f.Name == "PrevLocation");

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

		public static void Run(Action action)
		{
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
	}
}
