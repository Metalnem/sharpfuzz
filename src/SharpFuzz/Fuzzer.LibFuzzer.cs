using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime.InteropServices;

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
            public static void Run(ReadOnlySpanAction action) => Run(action, false);

            /// <summary>
            /// Run method starts the libFuzzer runner. It repeatedly executes
            /// the passed action and reports the execution result to libFuzzer.
            /// If the executable that is calling it is not running under libFuzzer,
            /// the action will be executed normally, and will receive its input
            /// from the file specified in the first command line parameter.
            /// </summary>
            /// <param name="action">
            /// Some action that calls the instrumented library. The span argument
            /// passed to the action contains the input data. Exceptions are not
            /// reported as crashes to libFuzzer (only timeouts and process crashes are).
            /// </param>
            public static void RunAndIgnoreExceptions(ReadOnlySpanAction action) => Run(action, true);

            private static unsafe void Run(ReadOnlySpanAction action, bool ignoreExceptions)
            {
                ThrowIfNull(action, nameof(action));

                try
                {
                    using (var ipc = new FuzzerIpc())
                    {
                        var sharedMem = ipc.InputPointer();
                        var trace = new TraceWrapper(sharedMem);

                        ipc.SetStatus(0);

                        try
                        {
                            var status = Fault.None;

                            // The program instrumented with libFuzzer will exit
                            // after the first error, so we should do the same.
                            while (status != Fault.Crash)
                            {
                                trace.ResetPrevLocation();

                                var size = ipc.InputSize();
                                var data = new ReadOnlySpan<byte>(sharedMem + MapSize, size);

                                try
                                {
                                    action(data);
                                }
                                catch (Exception ex)
                                {
                                    if (!ignoreExceptions)
                                    {
                                        Console.Error.WriteLine(ex);
                                        status = Fault.Crash;
                                    }
                                }

                                ipc.SetStatus(status);
                            }
                        }
                        catch
                        {
                            // Error communicating with the parent process, most likely
                            // because it was terminated after the timeout expired, or
                            // it was killed by the user. In any case, the exception
                            // details don't matter here, so we can just exit silently.
                            return;
                        }
                    }
                }
                catch (FuzzerIpcEnvironmentException)
                {
                    // Error establishing IPC with the parent process due to missing or
                    // definitely-invalid environment variables. This may be intentional.
                    // Instead of persistent fuzzing, fall back on testing a single input.
                    RunWithoutLibFuzzer(action);
                    return;
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
                    new TraceWrapper(sharedMem);
                    action(File.ReadAllBytes(args[1]));
                }
            }
        }
    }

    /// <summary>
    ///	  Cross-platform wrapper around an implementation of interprocess communication
    ///	  between SharpFuzz and a `libfuzzer-dotnet` process.
    /// </summary>
    class FuzzerIpc : IDisposable
    {
        IFuzzerIpcImpl impl;

        /// <summary>
        ///	  Attempt to initialize IPC for the current platform, using identifier data
        ///	  passed via environment variables.
        /// </summary>
        public FuzzerIpc()
        {
            var shmId = Environment.GetEnvironmentVariable("__LIBFUZZER_SHM_ID");
            var statusPipeId = Environment.GetEnvironmentVariable("__LIBFUZZER_STATUS_PIPE_ID");
            var controlPipeId = Environment.GetEnvironmentVariable("__LIBFUZZER_CONTROL_PIPE_ID");

            if (shmId == null || statusPipeId == null || controlPipeId == null)
            {
                throw new FuzzerIpcEnvironmentException();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                impl = new WindowsFuzzerIpc(shmId, statusPipeId, controlPipeId);
            }
            else
            {
                if (!Int32.TryParse(shmId, out var posixShmId))
                {
                    // The shared memory ID cannot be parsed as a 32-bit integer, which means it cannot
                    // have been the return value of `shmget(2)`, and cannot be passed to `shmat(2)`.
                    throw new FuzzerIpcEnvironmentException();
                }

                impl = new PosixFuzzerIpc(posixShmId, statusPipeId, controlPipeId);
            }
        }

        public FuzzerIpc(IFuzzerIpcImpl impl)
        {
            this.impl = impl;
        }

        public unsafe byte* InputPointer()
        {
            return impl.Pointer();
        }

        public int InputSize()
        {
            return impl.Control.ReadInt32();
        }

        public void SetStatus(int status)
        {
            impl.Status.Write(status);
        }

        public void Dispose()
        {
            impl.Dispose();
        }
    }

    /// <summary>
    ///	  Platform-specific implementation of interprocess communication between SharpFuzz
    ///	  and a `libfuzzer-dotnet` process.
    /// </summary>
    interface IFuzzerIpcImpl : IDisposable
    {
        public unsafe byte* Pointer();

        public BinaryReader Control { get; }

        public BinaryWriter Status { get; }
    }

    /// <summary>
    ///	  An error occurred when processing environment variables to initialize fuzzer IPC.
    ///	  For example, a required environment variable may not be set, or had a value that
    ///	  is definitely invalid.
    ///
    ///	  This exception does not imply an error when invoking platform APIs for setting up
    ///	  pipes or shared memory.
    /// </summary>
    class FuzzerIpcEnvironmentException : Exception
    {
    }

    /// <summary>
    ///	  IPC implementation for `libfuzzer-dotnet` on POSIX OS platforms.
    /// </summary>
    class PosixFuzzerIpc : IFuzzerIpcImpl
    {
        public BinaryReader Control { get; }
        public BinaryWriter Status { get; }

        private SharedMemoryHandle shmHandle;

        public PosixFuzzerIpc(int shmId, string statusPipeId, string controlPipeId)
        {
            Control = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, controlPipeId));
            Status = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, statusPipeId));

            shmHandle = Native.shmat(shmId, IntPtr.Zero, 0);
        }

        public unsafe byte* Pointer()
        {
            return (byte*)shmHandle.DangerousGetHandle();
        }

        public void Dispose()
        {
            Control.Dispose();
            Status.Dispose();
            shmHandle.Dispose();
        }
    }

    /// <summary>
    ///	  IPC implementation for `libfuzzer-dotnet` on Windows.
    /// </summary>
    class WindowsFuzzerIpc : IFuzzerIpcImpl
    {
        public BinaryReader Control { get; }
        public BinaryWriter Status { get; }

        private MemoryMappedFile mmFile;
        private MemoryMappedViewAccessor mmView;

        public WindowsFuzzerIpc(string shmId, string statusPipeId, string controlPipeId)
        {
            mmFile = MemoryMappedFile.OpenExisting(shmId, MemoryMappedFileRights.FullControl);
            mmView = mmFile.CreateViewAccessor();
            Control = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, controlPipeId));
            Status = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, statusPipeId));
        }

        public unsafe byte* Pointer()
        {
            byte* ptr = null;
            mmView.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            return ptr;
        }

        public void Dispose()
        {
            Control.Dispose();
            Status.Dispose();
            mmView.Dispose();
            mmFile.Dispose();
        }
    }
}
