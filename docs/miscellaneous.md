## Out-of-process fuzzing

SharpFuzz has several limitations compared to using
afl-fuzz directly with native programs. The first one
is that if you specify the timeout parameter, and the
timeout expires, the whole fuzzing process will be
terminated. The second one is that uncatchable exceptions
(**AccessViolationException** and **StackOverflowException**)
will also stop the fuzzing. In both cases, afl-fuzz will
terminate and display the following error message:

```
[-] PROGRAM ABORT : Unable to communicate with fork server (OOM?)
         Location : run_target(), afl-fuzz.c:2405
```

If you encounter this message during fuzzing, you can recover
the input data that has caused the premature exit from the file
```findings_dir/.cur_input```.

There is also an out-of-process version of fuzzer which is
using two different .NET processes: the master process for
the communication with afl-fuzz, and the child process for
the actual fuzzing. If the fuzzing process dies, the master
process will just restart it. This comes with the big
performance costs if the library you are testing throws
a lot of uncatchable exceptions, or timeouts often (starting
the new .NET process for each input takes a lot of time), so
it's best to fix such bugs as early as possible in order to
enjoy the best fuzzing performance. Using the out-of-process
fuzzer is as simple as replacing the call to **Fuzzer.Run**
with the call to **Fuzzer.OutOfProcess.Run**.

Another problem with the out-of-process fuzzer is that
the static constructors and all other types of static
initialization code are going to run again each time
the new child process is started, which will likely
negatively affect the trace bits.

## Test case minimization

AFL comes with the tool for test case minimization called
afl-tmin:

> **afl-tmin** is simple test case minimizer that
> takes an input file and tries to remove as
> much data as possible while keeping the binary
> in a crashing state or producing consistent
> instrumentation output (the mode is auto-selected
> based on initially observed behavior).

You can run it using the following command:

```shell
afl-tmin -i test_case -o minimized_result \
  dotnet path_to_assembly
```

The only change you have to make in your fuzzing
project is to replace the **Fuzzer.Run** call with
the call to **Fuzzer.RunOnce**.
