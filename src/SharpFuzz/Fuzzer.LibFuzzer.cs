using System;
using System.IO;
using System.IO.Pipes;

namespace SharpFuzz
{
	/// <summary>
	/// American fuzzy lop instrumentation and fork server for .NET libraries.
	/// </summary>
	public static partial class Fuzzer
	{
		/// <summary>
		/// Experimental implementation of libFuzzer runner.
		/// </summary>
		public static class LibFuzzer
		{
			/// <summary>
			/// Run method starts the libFuzzer runner. It repeatedly executes
			/// the passed action and reports the execution result to libFuzzer.
			/// This function will only work if the executable that is calling
			/// it is running under libFuzzer.
			/// </summary>
			/// <param name="action">
			/// Some action that calls the instrumented library. If an
			/// uncaught exception escapes the call, FAULT_CRASH execution
			/// status code is reported to libFuzzer.
			/// </param>
			public static unsafe void Run(ReadOnlySpanAction action)
			{
				ThrowIfNull(action, nameof(action));
				var s = Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID");

				if (s is null || !Int32.TryParse(s, out var shmid))
				{
					throw new Exception("This program can only be run under libFuzzer.");
				}

				using (var shmaddr = Native.shmat(shmid, IntPtr.Zero, 0))
				using (var r = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, "198")))
				using (var w = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, "199")))
				{
					byte* sharedMem = (byte*)shmaddr.DangerousGetHandle();
					InitializeSharedMemory(sharedMem);
					w.Write(0);

					while (true)
					{
						var size = r.ReadInt32();
						var data = new ReadOnlySpan<byte>(sharedMem + MapSize, size);
						var status = Fault.None;

						try
						{
							action(data);
						}
						catch
						{
							status = Fault.Crash;
						}

						w.Write(status);
					}
				}
			}
		}
	}
}
