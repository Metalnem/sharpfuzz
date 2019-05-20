## Using libFuzzer with SharpFuzz

SharpFuzz can now also be used with [libFuzzer] (currently
only on Linux, but Windows and macOS support is coming in
the near future). Unlike afl-fuzz, libFuzzer is under active
development. It also supports many advanced techniques that
are not possible with afl-fuzz, e.g. [structure-aware fuzzing].
Once all libFuzzer features are supported in SharpFuzz, it will
become the recommended fuzzing engine.

**1.** Instrumentation process remains the same as with afl-fuzz:

```shell
sharpfuzz path_to_assembly
```

**2.** In your fuzzing project, replace the **Fuzzer.Run**
call with the call to **Fuzzer.LibFuzzer.Run**.

**3.** Publish the project in order to generate the
self-contained executable:

```shell
dotnet publish -r linux-x64
```

**4.** Download the [libfuzzer-dotnet binary]. It was
confirmed to work on the following Linux distributions:

- Ubuntu 14.04 (Trusty)
- Ubuntu 16.04 (Xenial)
- Ubuntu 18.04 (Bionic)
- Ubuntu 18.10 (Cosmic)
- Ubuntu 19.04 (Disco)
- Debian 8 (Jessie)
- Debian 9 (Stretch)
- openSUSE Leap 42.3
- openSUSE Leap 15.0
- openSUSE Leap 15.1
- Fedora 29

**5.** Start the fuzzing with the following command:

```shell
./libfuzzer-dotnet --target_path=path_to_assembly testcases_dir
```

This is just the most basic way of using libFuzzer.
If you want to learn more about it, you should read
the [libFuzzer Tutorial].

[libFuzzer]: http://llvm.org/docs/LibFuzzer.html
[structure-aware fuzzing]: https://github.com/google/fuzzer-test-suite/blob/master/tutorial/structure-aware-fuzzing.md
[libfuzzer-dotnet binary]: https://github.com/Metalnem/libfuzzer-dotnet/releases/latest/download/libfuzzer-dotnet.tar.gz
[libFuzzer Tutorial]: https://github.com/google/fuzzer-test-suite/blob/master/tutorial/libFuzzerTutorial.md
