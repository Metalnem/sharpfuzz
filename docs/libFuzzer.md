## Using libFuzzer with SharpFuzz

You can use [libFuzzer] as a SharpFuzz fuzzing engine on Linux and Windows.

**1.** Download the latest [libfuzzer-dotnet] release for your platform.
Alternatively, you can compile [libfuzzer-dotnet.cc] (Linux) or
[libfuzzer-dotnet-windows.cc] (Windows) from scratch using the
following command:

```shell
clang -fsanitize=fuzzer libfuzzer-dotnet.cc -o libfuzzer-dotnet
```

**2.** In your **Main** function, call **Fuzzer.LibFuzzer.Run**
(instead of **Fuzzer.Run** or **Fuzzer.OutOfProcess.Run**).

**3.** Start fuzzing by running the [fuzz-libfuzzer.ps1] script like this:

```powershell
scripts/fuzz-libfuzzer.ps1 `
    -libFuzzer "libfuzzer-dotnet-windows.exe" `
    -project YourFuzzingProject.csproj `
    -corpus Testcases
```

[libFuzzer]: http://llvm.org/docs/LibFuzzer.html
[libfuzzer-dotnet]: https://github.com/Metalnem/libfuzzer-dotnet/releases
[libfuzzer-dotnet.cc]: https://github.com/Metalnem/libfuzzer-dotnet/blob/master/libfuzzer-dotnet.cc
[libfuzzer-dotnet-windows.cc]: https://github.com/Metalnem/libfuzzer-dotnet/blob/master/libfuzzer-dotnet-windows.cc
[fuzz-libfuzzer.ps1]: https://raw.githubusercontent.com/Metalnem/sharpfuzz/master/scripts/fuzz-libfuzzer.ps1
