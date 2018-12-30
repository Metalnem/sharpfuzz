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

**1.** Download the latest afl-fuzz [source package] and extract it.

**2.** Download the [RemoveInstrumentationCheck.diff] file and copy
it to the extracted directory (the directory is called **afl-2.52b**
in the current version of afl-fuzz).

**3.** Apply the patch by running the following command:

```shell
patch < RemoveInstrumentationCheck.diff
```

This step is necessary because alf-fuzz puts a magic string into
instrumented binaries in order to detect they are actually instrumented
before fuzzing them. Since we are going to run our programs with the
```dotnet run``` command, we have to tell afl-fuzz to skip the
instrumentation check, and that's exactly what this patch does.

**4.** Build the afl-fuzz by running the ```make``` command, or
install it by running the ```make install```command. If this
step fails, consult the **docs/INSTALL** document.

**5.** Install the [SharpFuzz.CommandLine] global .NET tool by
running the following command in your terminal:

```shell
dotnet tool install --global SharpFuzz.CommandLine --version 0.7.0
```

[source package]: http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
[RemoveInstrumentationCheck.diff]: https://github.com/Metalnem/sharpfuzz/raw/master/patches/RemoveInstrumentationCheck.diff
[SharpFuzz.CommandLine]: https://www.nuget.org/packages/SharpFuzz.CommandLine/

## Usage

**1.** If you are not already familiar with afl-fuzz, your first step
should be to read its documentation. Here are the most important links:

- [AFL quick start guide]
- [afl-fuzz README]
- [Understanding the status screen]

**2.** Choose the assembly you want to instrument. If you want to
instrument a NuGet package, you have to download it,
change its extension to **.zip**, and then extract it. The **.dll**
files for each supported platform will be located in the
**lib** directory (it's best to choose the **.dll** file found
in the directory corresponding to the latest version of the
.NET Standard, for example **lib/netstandard2.0**).

**3.** Instrument the assembly by running the following command:

```shell
sharpfuzz path_to_assembly
```

If the argument contains a path to some valid .NET assembly,
this command will most likely succeed. Otherwise, this
command could fail if the assembly has a reference to some
other library. For example, [Jil] package depends on [Sigil], and
if you attempt to instrument it, the command will fail with the following output:

> Assembly 'Sigil, Version=4.7.0.0, Culture=neutral, PublicKeyToken=2d06c3494341c8ab' is missing.
> Place it in the same directory as the assembly you want to instrument and then try again.

The message is pretty self-explanatory. Your next step would
be to download the missing NuGet dependency, find the **.dll** inside
it, and place it in the same directory as the assembly you
want to instrument.

**4.** Create a new .NET console project, and add the instrumented
library to it, along with all of its dependencies. You can do
that by adding the following element to your **.csproj** file (you
will have to change the hint path if the instrumented assembly
is not in the root directory of your project):

```xml
<ItemGroup>
  <Reference Include="Jil">
    <HintPath>Jil.dll</HintPath>
  </Reference>
</ItemGroup>
```

You can add the library dependencies the same way, but you
can also add them as NuGet package references.

**5.** Add the [SharpFuzz] package to the project by running
the following command:

```shell
dotnet add package SharpFuzz --version 0.7.0
```

**6.** Write the **Main** function so that it calls the
**SharpFuzz.Fuzzer.Run** with the function that you
want to test as a parameter. Taking Jil again as an
example, here is how such function might look:

```csharp
using System;
using System.IO;
using SharpFuzz;

namespace Jil.Fuzz
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Fuzzer.Run(() =>
      {
        using (var file = File.OpenText(args[0]))
        {
          JSON.DeserializeDynamic(file);
        }
      });
    }
  }
}
```

First important point here is that the path to the input
file being tested will be passed to your program by the
afl-fuzz as the first command line parameter, as you
can see in the example above.

Second important point is that if your code throws an
exception, it will be reported to afl-fuzz as a crash. If
you don't want to treat some particular exception as a crash,
catch it inside your function.

See [SharpFuzz.Samples] for dozens of examples
of complete fuzzing projects.

**7.** Create a directory for the test cases (one test is
almost always enough). Tests should contain some input
that is accepted by your code as valid, and should
also be as small as possible. For example, this is the
JSON file I'm using for testing JSON deserializers:

```json
{"menu":{"id":1,"val":"X","pop":{"a":[{"click":"Open()"},{"click":"Close()"}]}}}
```

**8.** You are now ready to go! Start the fuzzing with
the following command:

```shell
afl-fuzz -i testcases_dir -o findings_dir dotnet run @@
```

**9.** Wait! You will often have some useful results within minutes,
but sometimes it takes more than a day, so be patient. When an
unhandled exception happens, the input causing it will be added
to the **findings_dir/crashes** directory.

[AFL quick start guide]: http://lcamtuf.coredump.cx/afl/QuickStartGuide.txt
[afl-fuzz README]: http://lcamtuf.coredump.cx/afl/README.txt
[Understanding the status screen]: http://lcamtuf.coredump.cx/afl/status_screen.txt
[Jil]: https://www.nuget.org/packages/Jil/
[Sigil]: https://www.nuget.org/packages/Sigil/
[SharpFuzz]: https://www.nuget.org/packages/SharpFuzz
[SharpFuzz.Samples]: https://github.com/Metalnem/sharpfuzz/tree/master/src/SharpFuzz.Samples

## Limitations

SharpFuzz has several limitations compared to using
afl-fuzz directly with native programs. The first one
is that if you specify the timeout parameter, and the
timeout expires, the whole fuzzing process will be
terminated. The second one is that uncatchable exceptions
(**AccessViolationException** and **StackOverflowException**)
will also stop the fuzzing. The input that caused either
of these problems can be found in the file
**findings_dir/.cur_input**.

## Trophies

Coming soon!

## Resources

[american fuzzy lop (2.52b)](http://lcamtuf.coredump.cx/afl/)  
[go-fuzz: randomized testing for Go](https://github.com/dvyukov/go-fuzz)  
