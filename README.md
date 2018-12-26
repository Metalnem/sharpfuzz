# SharpFuzz: AFL-based fuzz testing for .NET

[![NuGet][nuget-shield]][nuget-link]
[![Build Status][build-shield]][build-link]
[![License][license-shield]][license-link]

[nuget-shield]: https://img.shields.io/nuget/v/SharpFuzz.svg
[nuget-link]: https://www.nuget.org/packages/SharpFuzz
[build-shield]: https://dev.azure.com/metalnem/sharpfuzz/_apis/build/status/Metalnem.sharpfuzz
[build-link]: https://dev.azure.com/metalnem/sharpfuzz/_build/latest?definitionId=2
[license-shield]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat
[license-link]: https://github.com/metalnem/sharpfuzz/blob/master/LICENSE

SharpFuzz is a tool that brings the power of [afl-fuzz]
to .NET platform. Technical blog post is coming up in
the following days, so follow my [blog] for more details!

[afl-fuzz]: http://lcamtuf.coredump.cx/afl/
[blog]: https://mijailovic.net/

## Requirements

afl-fuzz works on Linux and macOS. If you are using Windows,
you can use any Linux distribution that works under the
[Windows Subsystem for Linux].

You will also need to have the [.NET Core 2.1] or greater
installed on your machine.

[Windows Subsystem for Linux]: https://docs.microsoft.com/en-us/windows/wsl/install-win10
[.NET Core 2.1]: https://dotnet.microsoft.com/download

## Installation

1) Download the latest afl-fuzz [source package] and extract it.

2) Download the [RemoveInstrumentationCheck.diff] file and copy
it to the extracted directory (the directory is called **afl-2.52b**
in the current version of afl-fuzz).

3) Apply the patch by running the following command:

```shell
patch < RemoveInstrumentationCheck.diff
```

This step is necessary because alf-fuzz puts a magic string into
instrumented binaries in order to detect they are actually instrumented
before fuzzing them. Since we are going to run our programs with the
```dotnet run``` command, we have to tell afl-fuzz to skip the
instrumentation check, and that's exactly what this patch does.

4) Build the afl-fuzz by running the ```make``` command, or
install it by running the ```make install```command. If this
step fails, consult the **docs/INSTALL** document.

5) Install the [SharpFuzz.CommandLine] global .NET tool by
running the following command in your terminal:

```shell
dotnet tool install --global SharpFuzz.CommandLine --version 0.7.0
```

[source package]: http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
[RemoveInstrumentationCheck.diff]: https://github.com/Metalnem/sharpfuzz/raw/master/patches/RemoveInstrumentationCheck.diff
[SharpFuzz.CommandLine]: https://www.nuget.org/packages/SharpFuzz.CommandLine/

## Usage

Coming soon!
