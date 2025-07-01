using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace SharpFuzz
{
    /// <summary>
    /// American fuzzy lop instrumentation and fork server for .NET libraries.
    /// </summary>
    public static partial class Fuzzer
    {
        private const int MapSize = 1 << 16;
        private const int DefaultBufferSize = 10_000_000;

        /// <summary>
        /// Instrument method performs the in-place afl-fuzz
        /// instrumentation of the <paramref name="source"/> assembly.
        /// </summary>
        /// <param name="source">The assembly to instrument.</param>
        /// <param name="matcher">
        /// A function that accepts the full name of the class and returns
        /// true if the class should be instrumented, false otherwise.
        /// </param>
        /// <param name="options">
        /// Experimental options for controlling the instrumentation behavior.
        /// </param>
        /// <returns>An ordered collection of instrumented types.</returns>
        public static IEnumerable<string> Instrument(
            string source,
            Func<string, bool> matcher,
            Options options)
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
                        if (options.InstrumentMixedModeAssemblies
                            && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            // https://github.com/0xd4d/dnlib/issues/305
                            // https://github.com/0xd4d/dnlib/issues/499
                            src.Cor20HeaderFlags &= ~ComImageFlags.ILLibrary;
                        }
                        else
                        {
                            throw new InstrumentationException("Cannot instrument mixed-mode assemblies.");
                        }
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
                        types = Instrument(src, dst, matcher, options.EnableOnBranchCallback, traceType);
                    }
                    else
                    {
                        using (var commonMod = ModuleDefMD.Load(common.Location))
                        {
                            var traceType = commonMod.Types.Single(t => t.FullName == typeof(Common.Trace).FullName);
                            types = Instrument(src, dst, matcher, options.EnableOnBranchCallback, traceType);
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

        private static SortedSet<string> Instrument(
            ModuleDefMD src,
            Stream dst,
            Func<string, bool> matcher,
            bool enableOnBranchCallback,
            TypeDef traceType)
        {
            var sharedMemDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.SharedMem));
            var prevLocationDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.PrevLocation));
            var onBranchDef = traceType.Fields.Single(f => f.Name == nameof(Common.Trace.OnBranch));

            var sharedMemRef = src.Import(sharedMemDef);
            var prevLocationRef = src.Import(prevLocationDef);
            var onBranchRef = src.Import(onBranchDef);

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
                            Method.Instrument(sharedMemRef, prevLocationRef, onBranchRef, enableOnBranchCallback, method);

                            if (!instrumented)
                            {
                                types.Add(type.FullName);
                                instrumented = true;
                            }
                        }
                    }
                }
            }

            try
            {
                src.Write(dst);
            }
            catch (ModuleWriterException)
            {
                /*
                 * Likely if we got here it's because of obfuscation so we attempt to bypass it by keeping the old stack.
                 * If that doesn't solve it the error is likely to happen again which will bubble up to the user
                 */
                var writerOptions = new ModuleWriterOptions(src);
                writerOptions.MetadataOptions.Flags |= MetadataFlags.KeepOldMaxStack;
                src.Write(dst,writerOptions);
            } 
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

            var onBranchField = new FieldDefUser(
                nameof(Common.Trace.OnBranch),
                new FieldSig(mod.ImportAsTypeSig(typeof(Action<int, string>))),
                FieldAttributes.Public | FieldAttributes.Static
            );

            traceType.Fields.Add(sharedMemField);
            traceType.Fields.Add(prevLocationField);
            traceType.Fields.Add(onBranchField);

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
        /// execution result to afl-fuzz. If the executable that is
        /// calling it is not running under afl-fuzz, the action will
        /// be executed only once.
        /// </summary>
        /// <param name="action">
        /// Some action that calls the instrumented library. The stream
        /// argument passed to the action contains the input data. If an
        /// uncaught exception escapes the call, FAULT_CRASH execution
        /// status code is reported to afl-fuzz.
        /// </param>
        public static unsafe void Run(Action<Stream> action)
        {
            ThrowIfNull(action, nameof(action));
            var s = Environment.GetEnvironmentVariable("__AFL_SHM_ID");

            using (var stdin = Console.OpenStandardInput())
            using (var stream = new UnclosableStreamWrapper(stdin))
            {
                if (s is null || !Int32.TryParse(s, out var shmid))
                {
                    RunWithoutAflFuzz(action, stream);
                    return;
                }

                using (var shmaddr = Native.shmat(shmid, IntPtr.Zero, 0))
                using (var r = new BinaryReader(new AnonymousPipeClientStream(PipeDirection.In, "198")))
                using (var w = new BinaryWriter(new AnonymousPipeClientStream(PipeDirection.Out, "199")))
                {
                    var sharedMem = (byte*)shmaddr.DangerousGetHandle();
                    var trace = new TraceWrapper(sharedMem);

                    w.Write(0);
                    var pid = Process.GetCurrentProcess().Id;

                    using (var memory = new UnclosableStreamWrapper(new MemoryStream()))
                    {
                        // In the first run, we have to consume the input stream twice:
                        // first time to run the Setup function, second time to actually
                        // report the results. That's why we have to use the MemoryStream
                        // in order to be able to seek back to the beginning of the input.
                        stream.CopyTo(memory);
                        memory.Seek(0, SeekOrigin.Begin);

                        Setup(action, memory, sharedMem);
                        memory.Seek(0, SeekOrigin.Begin);

                        r.ReadInt32();
                        w.Write(pid);

                        trace.ResetPrevLocation();
                        w.Write(Execute(action, memory));
                    }

                    while (true)
                    {
                        r.ReadInt32();
                        w.Write(pid);

                        trace.ResetPrevLocation();
                        w.Write(Execute(action, stream));
                    }
                }
            }
        }

        /// <summary>
        /// Run method starts the .NET equivalent of AFL fork server.
        /// It repeatedly executes the passed action and reports the
        /// execution result to afl-fuzz. If the executable that is
        /// calling it is not running under afl-fuzz, the action will
        /// be executed only once.
        /// </summary>
        /// <param name="action">
        /// Some action that calls the instrumented library. The string
        /// argument passed to the action contains the input data. If an
        /// uncaught exception escapes the call, FAULT_CRASH execution
        /// status code is reported to afl-fuzz.
        /// </param>
        /// <param name="bufferSize">
        /// Optional size (in bytes) of the input buffer that will be used
        /// to read the whole stream before it's converted to a string. You
        /// should avoid using this parameter, unless fuzzer detects some
        /// interesting input that exceeds 10 MB (which is highly unlikely).
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if input data size in bytes exceeds <paramref name="bufferSize"/>.
        /// </exception>
        public static void Run(Action<string> action, int bufferSize = DefaultBufferSize)
        {
            Run(Wrap(action, bufferSize));
        }

        private static unsafe void RunWithoutAflFuzz(Action<Stream> action, Stream stream)
        {
            fixed (byte* sharedMem = new byte[MapSize])
            {
                new TraceWrapper(sharedMem);
                action(stream);
            }
        }

        /// <summary>
        /// RunOnce method executes the passed action once and writes the
        /// trace bits to the shared memory. This function will only work
        /// if the executable that is calling it is running under afl-fuzz.
        /// </summary>
        /// <param name="action">
        /// Some action that calls the instrumented library. The stream
        /// argument passed to the action contains the input data.
        /// </param>
        public static unsafe void RunOnce(Action<Stream> action)
        {
            ThrowIfNull(action, nameof(action));
            var s = Environment.GetEnvironmentVariable("__AFL_SHM_ID");

            if (s is null || !Int32.TryParse(s, out var shmid))
            {
                throw new Exception("This program can only be run under afl-fuzz.");
            }

            using (var stdin = Console.OpenStandardInput())
            using (var stream = new UnclosableStreamWrapper(stdin))
            using (var shmaddr = Native.shmat(shmid, IntPtr.Zero, 0))
            {
                new TraceWrapper((byte*)shmaddr.DangerousGetHandle());
                action(stream);
            }
        }

        // Initial run will usually have the different trace bits
        // from the subsequent runs because static constructors
        // and other types of static initialization are going to
        // be executed only once. To prevent "WARNING: Instrumentation
        // output varies across runs" message in the afl-fuzz, we can
        // safely ignore the first execution during the dry run.
        private static unsafe void Setup(Action<Stream> action, Stream stream, byte* sharedMem)
        {
            Execute(action, stream);
            new Span<byte>(sharedMem, MapSize).Clear();
        }

        private static int Execute(Action<Stream> action, Stream stream)
        {
            try
            {
                action(stream);
            }
            catch
            {
                return Fault.Crash;
            }

            return Fault.None;
        }

        private static Action<Stream> Wrap(Action<string> action, int bufferSize)
        {
            var buffer = new byte[Math.Max(bufferSize, DefaultBufferSize)];

            return stream =>
            {
                var read = stream.Read(buffer, 0, buffer.Length);

                if (read == buffer.Length)
                {
                    throw new InvalidOperationException($"Input data size must not exceed {bufferSize} bytes.");
                }

                action(Encoding.UTF8.GetString(buffer, 0, read));
            };
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
