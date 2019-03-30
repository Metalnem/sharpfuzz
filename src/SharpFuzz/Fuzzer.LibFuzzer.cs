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
		/// LibFuzzer class contains the libFuzzer runner. It enables users
		/// to fuzz their code with libFuzzer by using the libFuzzer-dotnet
		/// binary, which acts as a bridge between the libFuzzer and the
		/// managed code (it currently works only on Linux).
		/// </summary>
		public static class LibFuzzer
		{
			/// <summary>
			/// Run method starts the libFuzzer runner. It repeatedly executes
			/// the passed action and reports the execution result to libFuzzer.
			/// If the executable that is calling it is not running under libFuzzer,
			/// the action will be executed normally, and will receive its input
			/// from the file specified in the first command line parameter.
			/// </summary>
			/// <param name="action">
			/// Some action that calls the instrumented library. The span argument
			/// passed to the action contains the input data. If an uncaught
			/// exception escapes the call, crash is reported to libFuzzer.
			/// </param>
			public static unsafe void Run(ReadOnlySpanAction action)
			{
				ThrowIfNull(action, nameof(action));
				var s = Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID");

				if (s is null || !Int32.TryParse(s, out var shmid))
				{
					RunWithoutLibFuzzer(action);
					return;
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

						try
						{
							action(data);
							w.Write(Fault.None);
						}
						catch (Exception ex)
						{
							Console.Error.WriteLine(ex);
							w.Write(Fault.Crash);

							// The program instrumented with libFuzzer will exit
							// after the first error, so we should do the same.
							return;
						}
					}
				}
			}

			private static unsafe void RunWithoutLibFuzzer(ReadOnlySpanAction action)
			{
				var args = Environment.GetCommandLineArgs();

				if (args.Length <= 1)
				{
					Console.Error.WriteLine("You must specify the input path as the first command line argument when not running under libFuzzer.");
				}

				fixed (byte* sharedMem = new byte[MapSize])
				{
					InitializeSharedMemory(sharedMem);
					action(File.ReadAllBytes(args[1]));
				}
			}
		}
	}
}
