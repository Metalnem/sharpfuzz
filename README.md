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

AFL works on Linux and macOS. If you are using
Windows, you can use any Linux distribution
that works under the [Windows Subsystem for Linux].

You will need GNU make and a working compiler
(gcc or clang) in order to compile afl-fuzz.
You will also need to have the [.NET Core 2.1]
or greater installed on your machine in order
to instrument .NET assemblies with SharpFuzz.

[Windows Subsystem for Linux]: https://docs.microsoft.com/en-us/windows/wsl/install-win10
[.NET Core 2.1]: https://dotnet.microsoft.com/download

## Installation

You can install afl-fuzz and [SharpFuzz.CommandLine]
global .NET tool by running the following [script]:

```shell
#/bin/sh
set -eux

# Download and extract the latest afl-fuzz source package
wget http://lcamtuf.coredump.cx/afl/releases/afl-latest.tgz
tar -xvf afl-latest.tgz

rm afl-latest.tgz
cd afl-2.52b/

# Patch afl-fuzz so that it doesn't check whether the binary
# being fuzzed is instrumented (we have to do this because
# we are going to run our programs with the dotnet run command,
# and the dotnet binary would fail this check)
wget https://github.com/Metalnem/sharpfuzz/raw/master/patches/RemoveInstrumentationCheck.diff
patch < RemoveInstrumentationCheck.diff

# Install afl-fuzz
make install
rm -rf afl-2.52b/

# Install SharpFuzz.CommandLine global .NET tool
dotnet tool install --global SharpFuzz.CommandLine --version 0.7.0
```

[SharpFuzz.CommandLine]: https://www.nuget.org/packages/SharpFuzz.CommandLine/
[script]: https://github.com/Metalnem/sharpfuzz/raw/master/build/Install.sh

## Usage

This tutorial assumes that you are somewhat familiar
with afl-fuzz. If you don't know anything about it, you
should first read the [AFL quick start guide] and the
[afl-fuzz README]. If you have enough time, I would
also recommend reading [Understanding the status screen]
and [Technical whitepaper for afl-fuzz].

[AFL quick start guide]: http://lcamtuf.coredump.cx/afl/QuickStartGuide.txt
[afl-fuzz README]: http://lcamtuf.coredump.cx/afl/README.txt
[Understanding the status screen]: http://lcamtuf.coredump.cx/afl/status_screen.txt
[Technical whitepaper for afl-fuzz]: http://lcamtuf.coredump.cx/afl/technical_details.txt

As an example, we are going to instrument [Jil],
which is a fast JSON serializer and deserializer
(see [SharpFuzz.Samples] for many more examples
of complete fuzzing projects).

[Jil]: https://www.nuget.org/packages/Jil/
[SharpFuzz.Samples]: https://github.com/Metalnem/sharpfuzz/tree/master/src/SharpFuzz.Samples

**1.** Download the package from the NuGet gallery.
You can do that by clicking the [download package]
link in the info section of the page. The downloaded
file will be called ```jil.2.16.0.nupkg```.

**2.** Change the extension of the downloaded file
from **nupkg** to **zip**, and then extract it.
The location of the assembly we are going to instrument
will be ```jil.2.16.0/lib/netstandard2.0/Jil.dll```.
We could have chosen some other .NET platform, such
as **net45** or **netstandard1.6**, but the latest
version of .NET Standard is usually the best choice.

**3.** Instrument the assembly by running the
```sharpfuzz``` tool with the path to the assembly
as a parameter. In our case, the exact command looks
like this:

```shell
sharpfuzz jil.2.16.0/lib/netstandard2.0/Jil.dll
```

This will work most of the time, and the assembly
will be overwritten with the instrumented version.

If the specified assembly has a reference to some
other library, this command will fail. Jil depends
on [Sigil], which is why you will see the following
error after attempting to instrument it:

> Assembly 'Sigil, Version=4.7.0.0, Culture=neutral, PublicKeyToken=2d06c3494341c8ab' is missing.
> Place it in the same directory as the assembly you want to instrument and then try again.

This means you will have to download and
extract the Sigil package from NuGet, copy
```sigil.4.7.0/lib/netstandard1.5/Sigil.dll```
to ```jil.2.16.0/lib/netstandard2.0```, and
then run ```sharpfuzz``` again. This time
everything should work fine, and
```jil.2.16.0/lib/netstandard2.0/Jil.dll```
will contain the instrumented version of Jil.

**4.** Create a new .NET console project, and add
the instrumented library to it, along with all of
its dependencies. To do that, copy both ```Jil.dll```
and ```Sigil.dll``` to the root directory of the
project, and then add the following element to
your project file:

```xml
<ItemGroup>
  <Reference Include="Jil">
    <HintPath>Jil.dll</HintPath>
  </Reference>

  <Reference Include="Sigil">
    <HintPath>Sigil.dll</HintPath>
  </Reference>
</ItemGroup>
```

**5.** Add the [SharpFuzz] package to the project by running
the following command:

```shell
dotnet add package SharpFuzz --version 0.7.0
```

**6.** Now it's time to write some code. The **Main**
function should call the **SharpFuzz.Fuzzer.Run**
with the function that we want to test as a parameter.
Here's the one possible way we could write this:

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
        try
        {
          using (var file = File.OpenText(args[0]))
          {
            JSON.DeserializeDynamic(file);
          }
        }
        catch (DeserializationException) { }
      });
    }
  }
}
```

We want to fuzz the deserialization capabilities of Jil,
which is why we are calling the **JSON.DeserializeDynamic**
method. The path to the input file being tested will always
be provided to our program as the first command line argument
(afl-fuzz will take care of that during the fuzzing process).

If the code passed to **Fuzzer.Run** throws an exception,
it will be reported to afl-fuzz as a crash. However, we
want to treat only *unexpected* exceptions as bugs.
**DeserializationException** is what we expect when
we encounter an invalid JSON input, which is why we
catch it in our example.

**7.** Create a directory with some test cases (one
test is usually more than enough). Test files
should contain some input that is accepted by
your code as valid, and should also be as small as
possible. For example, this is the JSON I'm using
for testing JSON deserializers:

```json
{"menu":{"id":1,"val":"X","pop":{"a":[{"click":"Open()"},{"click":"Close()"}]}}}
```

**8.** You are now ready to go! Build the project
with ```dotnet build```, and start the fuzzing with
the following command:

```shell
afl-fuzz -i testcases_dir -o findings_dir \
  dotnet path_to_assembly @@
```

Let's say that our working directory is called ```Fuzzing```.
If it contains the project ```Fuzzing.csproj```, and the
directory called ```Testcases```, the full command might
look like this:

```shell
afl-fuzz -i Testcases -o Findings \
  dotnet bin/Debug/netcoreapp2.1/Fuzzing.dll @@
```

For formats such as HTML, JavaScript, JSON, or SQL,
the fuzzing process can be greatly improved with
the usage of a [dictionary] file. AFL comes with
bunch of dictionaries, which you can find after
installation in ```/usr/local/share/afl/dictionaries/```.
With this in mind, we can improve our fuzzing of Jil like this:

```shell
afl-fuzz -i Testcases -o Findings \
  -x /usr/local/share/afl/dictionaries/json.dict \
  dotnet bin/Debug/netcoreapp2.1/Fuzzing.dll @@
```

**9.** Sit back and relax! You will often have
some useful results within minutes, but sometimes
it can take more than a day, so be patient.

The input files responsible for unhandled exceptions
will appear in ```findings_dir/crashes```. The total
number of unique crashes will be displayed in red on
the afl-fuzz status screen.

[download package]: https://www.nuget.org/api/v2/package/Jil/2.16.0
[Sigil]: https://www.nuget.org/packages/Sigil/
[SharpFuzz]: https://www.nuget.org/packages/SharpFuzz
[dictionary]: https://lcamtuf.blogspot.com/2015/01/afl-fuzz-making-up-grammar-with.html

## Limitations

SharpFuzz has several limitations compared to using
afl-fuzz directly with native programs. The first one
is that if you specify the timeout parameter, and the
timeout expires, the whole fuzzing process will be
terminated. The second one is that uncatchable exceptions
(**AccessViolationException** and **StackOverflowException**)
will also stop the fuzzing. However, the input that has
caused either of these problems is not lost, and it can
be found in the file ```findings_dir/.cur_input```.

## Trophies

Coming soon!

## Resources

[american fuzzy lop (2.52b)](http://lcamtuf.coredump.cx/afl/)  
[go-fuzz: randomized testing for Go](https://github.com/dvyukov/go-fuzz)  
