# SharpFuzz: AFL-based fuzz testing for .NET

[![NuGet][nuget-shield]][nuget-link]
[![Build Status][build-shield]][build-link]
[![License][license-shield]][license-link]

[nuget-shield]: https://img.shields.io/nuget/v/SharpFuzz.svg
[nuget-link]: https://www.nuget.org/packages/SharpFuzz
[build-shield]: https://ci.appveyor.com/api/projects/status/tpufk0k8h4falflf?svg=true
[build-link]: https://ci.appveyor.com/project/Metalnem/sharpfuzz
[license-shield]: https://img.shields.io/badge/license-MIT-blue.svg?style=flat
[license-link]: https://github.com/metalnem/sharpfuzz/blob/master/LICENSE

SharpFuzz is a tool that brings the power of [afl-fuzz]
to .NET platform. If you want to learn more about fuzzing,
my motivation for writing SharpFuzz, the types of bugs
it can find, or the technical details about how the
integration with afl-fuzz works, read my blog post
[SharpFuzz: Bringing the power of afl-fuzz to .NET platform](https://mijailovic.net/2019/01/03/sharpfuzz/).

[afl-fuzz]: http://lcamtuf.coredump.cx/afl/

## Table of contents

- [CVE](#cve)
- [Trophies](#trophies)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Advanced topics](#advanced-topics)
- [Acknowledgements](#acknowledgements)

## CVE

- [CVE-2019-0980: .NET Framework and .NET Core Denial of Service Vulnerability](https://portal.msrc.microsoft.com/en-us/security-guidance/advisory/CVE-2019-0980)
- [CVE-2019-0981: .NET Framework and .NET Core Denial of Service Vulnerability](https://portal.msrc.microsoft.com/en-us/security-guidance/advisory/CVE-2019-0981)

## Trophies

If you find some interesting bugs with SharpFuzz, and
are comfortable with sharing them, I would love to add
them to this list. Please send me an email, make a pull
request for the README file, or file an issue.
  
- [AngleSharp: HtmlParser.Parse throws InvalidOperationException](https://github.com/AngleSharp/AngleSharp/issues/735) **fixed**
- [CoreFX: BigInteger.TryParse out-of-bounds access](https://github.com/dotnet/corefx/issues/35176) **fixed**
- [CoreFX: BinaryFormatter.Deserialize throws many unexpected exceptions](https://github.com/dotnet/corefx/issues/35491)
- [CoreFX: DataContractJsonSerializer.ReadObject throws ArgumentOutOfRangeException](https://github.com/dotnet/corefx/issues/35205)
- [CoreFX: DataContractJsonSerializer.ReadObject throws IndexOutOfRangeException](https://github.com/dotnet/corefx/issues/35204)
- [CoreFX: DataContractSerializer.ReadObject throws ArgumentNullException](https://github.com/dotnet/corefx/issues/35203)
- [CoreFX: Double.Parse throws AccessViolationException on .NET Core 3.0](https://github.com/dotnet/corefx/issues/35780) **fixed**
- [CoreFX: G17 format specifier doesn't always round-trip double values](https://github.com/dotnet/corefx/issues/35369)
- [CoreFX: Uri.TryCreate throws IndexOutOfRangeException](https://github.com/dotnet/corefx/issues/35072)
- [CoreFX: XmlReader.Create throws IndexOutOfRangeException](https://github.com/dotnet/corefx/issues/35073) **fixed**
- [DotLiquid: Template.Parse throws ArgumentNullException instead of SyntaxException](https://github.com/dotliquid/dotliquid/issues/333)
- [Esprima .NET: JavaScriptParser.ParseProgram throws ArgumentOutOfRangeException](https://github.com/sebastienros/esprima-dotnet/issues/70)
- [Esprima .NET: StackOverflowException when parsing a lot of starting parentheses](https://github.com/sebastienros/esprima-dotnet/issues/104) **fixed**
- [ExcelDataReader: ExcelReaderFactory.CreateBinaryReader can throw unexpected exceptions](https://github.com/ExcelDataReader/ExcelDataReader/issues/383) **fixed**
- [ExcelDataReader: ExcelReaderFactory.CreateBinaryReader throws OutOfMemoryException](https://github.com/ExcelDataReader/ExcelDataReader/issues/382) **fixed**
- [ExCSS: StylesheetParser.Parse throws ArgumentOutOfRangeException](https://github.com/TylerBrinks/ExCSS/issues/101)
- [Google.Protobuf: MessageParser.ParseFrom throws unexpected exceptions (C#)](https://github.com/protocolbuffers/protobuf/issues/5513) **fixed**
- [GraphQL-Parser: Parser.Parse takes around 18s to parse the 58K file](https://github.com/graphql-dotnet/parser/issues/22) **fixed**
- [GraphQL-Parser: Parser.Parse throws ArgumentOutOfRangeException](https://github.com/graphql-dotnet/parser/issues/21) **fixed**
- [Handlebars.Net: Handlebars.Compile hangs permanently](https://github.com/rexm/Handlebars.Net/issues/283) **fixed**
- [Handlebars.Net: Template engine throws some unexpected exceptions](https://github.com/rexm/Handlebars.Net/issues/282) **fixed**
- [Jil: JSON.DeserializeDynamic throws ArgumentException](https://github.com/kevin-montrose/Jil/issues/316) **fixed**
- [Jint: Engine.Execute can throw many unexpected exceptions](https://github.com/sebastienros/jint/issues/571) **fixed**
- [Jint: Engine.Execute takes more than two minutes to complete (even with the 2s timeout)](https://github.com/sebastienros/jint/issues/586) **fixed**
- [Jint: Engine.Execute throws OutOfMemoryException after 45s (even with the 2s timeout)](https://github.com/sebastienros/jint/issues/587) **fixed**
- [Json.NET: JsonConvert.DeserializeObject can throw several unexpected exceptions](https://github.com/JamesNK/Newtonsoft.Json/issues/1947)
- [Jurassic: ScriptEngine.Execute terminates the process with StackOverflowException](https://github.com/paulbartrum/jurassic/issues/141)
- [Jurassic: ScriptEngine.Execute throws some unexpected exceptions](https://github.com/paulbartrum/jurassic/issues/142) **fixed**
- [Jurassic: ScriptEngine.ExecuteFile hangs permanently instead of throwing JavaScriptException](https://github.com/paulbartrum/jurassic/issues/138) **fixed**
- [Jurassic: ScriptEngine.ExecuteFile throws FormatException](https://github.com/paulbartrum/jurassic/issues/137) **fixed**
- [LumenWorks CSV Reader: CsvReader.ReadNextRecord throws IndexOutOfRangeException](https://github.com/phatcher/CsvReader/issues/67)
- [Markdig: Markdown.ToHtml hangs permanently](https://github.com/lunet-io/markdig/issues/278) **fixed**
- [Markdig: Markdown.ToHtml takes more than two minutes to complete when processing the 32K file](https://github.com/lunet-io/markdig/issues/306) **fixed**
- [Markdig: Markdown.ToHtml throws ArgumentOutOfRangeException](https://github.com/lunet-io/markdig/issues/275) **fixed**
- [Markdig: Markdown.ToHtml throws IndexOutOfRangeException](https://github.com/lunet-io/markdig/issues/276) **fixed**
- [Markdig: Markdown.ToHtml throws IndexOutOfRangeException](https://github.com/lunet-io/markdig/issues/303)
- [Markdig: Markdown.ToHtml throws NullReferenceException](https://github.com/lunet-io/markdig/issues/277) **fixed**
- [MarkdownSharp: Markdown.Transform hangs permanently](https://github.com/StackExchange/MarkdownSharp/issues/8)
- [MessagePack for CLI: Unpacking.UnpackObject throws several unexpected exceptions](https://github.com/msgpack/msgpack-cli/issues/311)
- [Mono.Cecil: ModuleDefinition.ReadModule can throw many (possibly) unexpected exceptions](https://github.com/jbevain/cecil/issues/556)
- [Mono.Cecil: ModuleDefinition.ReadModule hangs permanently](https://github.com/jbevain/cecil/issues/555) **fixed**
- [NCrontab: CrontabSchedule.Parse throws OverflowException instead of CrontabException](https://github.com/atifaziz/NCrontab/issues/43)
- [NUglify: Uglify.Js hangs permanently](https://github.com/xoofx/NUglify/issues/63)
- [Open XML SDK: Add some security/fuzz testing](https://github.com/OfficeDev/Open-XML-SDK/issues/441)
- [OpenMCDF: OutOfMemoryException when parsing Excel document / endless while-loop](https://github.com/ironfede/openmcdf/issues/30) **fixed**
- [OpenMCDF: System.ArgumentOutOfRangeException take 2](https://github.com/ironfede/openmcdf/issues/39) **fixed**
- [OpenMCDF: System.ArgumentOutOfRangeException when trying to open certain invalid files](https://github.com/ironfede/openmcdf/issues/38) **fixed**
- [OpenMCDF: System.OutOfMemoryException when reading corrupt Word document](https://github.com/ironfede/openmcdf/issues/40) **fixed**
- [PdfPig: StackOverflowException reading corrupt PDF document](https://github.com/UglyToad/PdfPig/issues/33) **fixed**
- [protobuf-net: Serializer.Deserialize can throw many unexpected exceptions](https://github.com/mgravell/protobuf-net/issues/481)
- [protobuf-net: Serializer.Deserialize hangs permanently](https://github.com/mgravell/protobuf-net/issues/479)
- [Scriban: Template.ParseLiquid throws ArgumentOutOfRangeException](https://github.com/lunet-io/scriban/issues/121) **fixed**
- [Scriban: Template.ParseLiquid throws NullReferenceException](https://github.com/lunet-io/scriban/issues/120) **fixed**
- [Scriban: Template.Render throws InvalidCastException](https://github.com/lunet-io/scriban/issues/122) **fixed**
- [SharpCompress: Enumerating ZipArchive.Entries collection throws NullReferenceException](https://github.com/adamhathcock/sharpcompress/issues/431)
- [SharpZipLib: ZipInputStream.GetNextEntry hangs permanently](https://github.com/icsharpcode/SharpZipLib/issues/300)
- [SixLabors.Fonts: FontDescription.LoadDescription throws ArgumentException](https://github.com/SixLabors/Fonts/issues/96) **fixed**
- [SixLabors.Fonts: FontDescription.LoadDescription throws NullReferenceException](https://github.com/SixLabors/Fonts/issues/97) **fixed**
- [SixLabors.ImageSharp: Image.Load terminates the process with AccessViolationException](https://github.com/SixLabors/ImageSharp/issues/798) **fixed**
- [SixLabors.ImageSharp: Image.Load throws AccessViolationException](https://github.com/SixLabors/ImageSharp/issues/827) **fixed**
- [SixLabors.ImageSharp: Image.Load throws ArgumentException](https://github.com/SixLabors/ImageSharp/issues/826) **fixed**
- [SixLabors.ImageSharp: Image.Load throws ArgumentOutOfRangeException](https://github.com/SixLabors/ImageSharp/issues/825) **fixed**
- [SixLabors.ImageSharp: Image.Load throws DivideByZeroException](https://github.com/SixLabors/ImageSharp/issues/821) **fixed**
- [SixLabors.ImageSharp: Image.Load throws DivideByZeroException](https://github.com/SixLabors/ImageSharp/issues/822) **fixed**
- [SixLabors.ImageSharp: Image.Load throws ExecutionEngineException](https://github.com/SixLabors/ImageSharp/issues/839) **fixed**
- [SixLabors.ImageSharp: Image.Load throws IndexOutOfRangeException](https://github.com/SixLabors/ImageSharp/issues/824) **fixed**
- [SixLabors.ImageSharp: Image.Load throws NullReferenceException](https://github.com/SixLabors/ImageSharp/issues/797) **fixed**
- [SixLabors.ImageSharp: Image.Load throws NullReferenceException](https://github.com/SixLabors/ImageSharp/issues/823) **fixed**
- [Utf8Json: JsonSerializer.Deserialize can throw many unexpected exceptions](https://github.com/neuecc/Utf8Json/issues/142)
- [Web Markup Minifier: HtmlMinifier.Minify hangs permanently](https://github.com/Taritsyn/WebMarkupMin/issues/73) **fixed**
- [Web Markup Minifier: HtmlMinifier.Minify throws InvalidOperationException](https://github.com/Taritsyn/WebMarkupMin/issues/77) **fixed**
- [YamlDotNet: YamlStream.Load takes more than 60s to parse the 37K file](https://github.com/aaubry/YamlDotNet/issues/379)
- [YamlDotNet: YamlStream.Load terminates the process with StackOverflowException](https://github.com/aaubry/YamlDotNet/issues/375)
- [YamlDotNet: YamlStream.Load throws ArgumentException](https://github.com/aaubry/YamlDotNet/issues/374)

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
cd ..
rm -rf afl-2.52b/

# Install SharpFuzz.CommandLine global .NET tool
dotnet tool install --global SharpFuzz.CommandLine
```

The alternative to patching afl-fuzz in order to skip the
instrumentation check is to set the **AFL_SKIP_BIN_CHECK**
environment variable.

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
[SharpFuzz.Samples]: https://github.com/Metalnem/sharpfuzz-samples

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

The instrumentation is performed in place, which
means that ```jil.2.16.0/lib/netstandard2.0/Jil.dll```
will contain the instrumented version of Jil after
running this command.

**4.** Create a new .NET console project, and add
the instrumented library to it, along with all of
its dependencies. To do that, copy ```Jil.dll```
to the root directory of the project, and then add
the following element to your project file:

```xml
<ItemGroup>
  <Reference Include="Jil">
    <HintPath>Jil.dll</HintPath>
  </Reference>
</ItemGroup>
```

Jil depends on [Sigil], which is why you also have to
manually add the reference to Sigil. You can install it
from NuGet with the following command:

```shell
dotnet add package Sigil --version 4.7.0
```

**5.** Add the [SharpFuzz] package to the project by running
the following command:

```shell
dotnet add package SharpFuzz
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
      Fuzzer.Run(stream =>
      {
        try
        {
          using (var reader = new StreamReader(stream))
          {
            JSON.DeserializeDynamic(reader);
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
method. The input data will be be provided to us via the
**stream** parameter (if the code you are testing takes
its input as a string, you can use an additional overload
of **Fuzzer.Run** that accepts **Action&lt;string&gt;**).

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
  dotnet path_to_assembly
```

Let's say that our working directory is called ```Fuzzing```.
If it contains the project ```Fuzzing.csproj```, and the
directory called ```Testcases```, the full command might
look like this:

```shell
afl-fuzz -i Testcases -o Findings \
  dotnet bin/Debug/netcoreapp2.1/Fuzzing.dll
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
  dotnet bin/Debug/netcoreapp2.1/Fuzzing.dll
```

Sometimes you may encounter the following error when
running afl-fuzz:

```
[-] Oops, the program crashed with one of the test cases provided. There are
    several possible explanations:
```

This usually happens when some of your provided test
inputs cause the fuzzing function to throw an exception,
but sometimes this can happen due to low default memory
limit (I see this very often in the cloud environment).
You can fix it by increasing the memory limit for your
program to some large value:

```shell
afl-fuzz -i testcases_dir -o findings_dir -m 10000 \
  dotnet path_to_assembly
```

**9.** Sit back and relax! You will often have
some useful results within minutes, but sometimes
it can take more than a day, so be patient.

The input files responsible for unhandled exceptions
will appear in ```findings_dir/crashes```. The total
number of unique crashes will be displayed in red on
the afl-fuzz status screen.

In practice, the real number of unique exceptions will often
be much lower than the reported number, which is why it's
usually best to write a small program that just goes through
the crashing inputs, runs the fuzzing function on each of
them, and saves only the inputs that produce unique stack traces.

[download package]: https://www.nuget.org/api/v2/package/Jil/2.16.0
[Sigil]: https://www.nuget.org/packages/Sigil/
[SharpFuzz]: https://www.nuget.org/packages/SharpFuzz
[dictionary]: https://lcamtuf.blogspot.com/2015/01/afl-fuzz-making-up-grammar-with.html

## Advanced topics

- [Fuzzing .NET Core](https://github.com/Metalnem/sharpfuzz/blob/master/docs/fuzzing-dotnet-core.md)
- [Out-of-process fuzzing](https://github.com/Metalnem/sharpfuzz/blob/master/docs/miscellaneous.md#out-of-process-fuzzing)
- [Test case minimization](https://github.com/Metalnem/sharpfuzz/blob/master/docs/miscellaneous.md#test-case-minimization)
- [Using libFuzzer with SharpFuzz](https://github.com/Metalnem/sharpfuzz/blob/master/docs/libFuzzer.md)

## Acknowledgements

- **Michal Zalewski** - [american fuzzy lop](http://lcamtuf.coredump.cx/afl/)
- **Dmitry Vyukov** - [go-fuzz: randomized testing for Go](https://github.com/dvyukov/go-fuzz)
- **Rody Kersten** - [Kelinci: AFL-based fuzzing for Java](https://github.com/isstac/kelinci)
- **Jb Evain** - [Mono.Cecil](https://github.com/jbevain/cecil)
- **0xd4d** - [dnlib](https://github.com/0xd4d/dnlib)
- **Guido Vranken** - [go-fuzz: libFuzzer support](https://github.com/dvyukov/go-fuzz/pull/217)
