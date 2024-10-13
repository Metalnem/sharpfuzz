# SharpFuzz: AFL-based fuzz testing for .NET

[![NuGet][nuget-shield]][nuget-link]
[![Build Status][build-shield]][build-link]
[![License][license-shield]][license-link]

[nuget-shield]: https://img.shields.io/nuget/v/SharpFuzz.svg
[nuget-link]: https://www.nuget.org/packages/SharpFuzz
[build-shield]: https://github.com/metalnem/sharpfuzz/actions/workflows/dotnet.yml/badge.svg
[build-link]: https://github.com/Metalnem/sharpfuzz/actions/workflows/dotnet.yml
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
- [Articles](#articles)
- [Trophies](#trophies)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Advanced topics](#advanced-topics)
- [Acknowledgements](#acknowledgements)

## CVE

- [CVE-2019-0980: .NET Framework and .NET Core Denial of Service Vulnerability](https://portal.msrc.microsoft.com/en-us/security-guidance/advisory/CVE-2019-0980)
- [CVE-2019-0981: .NET Framework and .NET Core Denial of Service Vulnerability](https://portal.msrc.microsoft.com/en-us/security-guidance/advisory/CVE-2019-0981)

## Articles

- [SharpFuzz: Bringing the power of afl-fuzz to .NET platform](https://mijailovic.net/2019/01/03/sharpfuzz/)
- [Five years of fuzzing .NET with SharpFuzz](https://mijailovic.net/2023/07/23/sharpfuzz-anniversary/)
- [Let’s do DHCP: fuzzing](http://writeasync.net/?p=5714)
- [Fuzzing C# on Windows with SharpFuzz and libfuzzer-dotnet](https://github.com/ranweiler/libfuzzer-dotnet-windows-example/blob/main/README.md)
- [Automate Bug Finding: Fuzzing C# Code on Windows](https://blog.objektkultur.de/Automate-Bug-Finding-Fuzzing-C-Sharp-Code-on-Windows/)

## Trophies

If you find some interesting bugs with SharpFuzz, and
are comfortable with sharing them, I would love to add
them to this list. Please send me an email, make a pull
request for the README file, or file an issue.
  
- [AngleSharp: HtmlParser.Parse throws InvalidOperationException](https://github.com/AngleSharp/AngleSharp/issues/735) **fixed**
- [AngleSharp: HtmlParser.ParseDocument throws IndexOutOfRangeException](https://github.com/AngleSharp/AngleSharp/issues/1174) **fixed**
- [AngleSharp: HtmlParser.ParseDocument throws InvalidOperationException](https://github.com/AngleSharp/AngleSharp/issues/1176) **fixed**
- [AngleSharp: HtmlParser.ParseDocument hangs permanently](https://github.com/AngleSharp/AngleSharp/issues/1179) **fixed**
- [CoreFX: BigInteger.TryParse out-of-bounds access](https://github.com/dotnet/corefx/issues/35176) **fixed**
- [CoreFX: BinaryFormatter.Deserialize throws many unexpected exceptions](https://github.com/dotnet/corefx/issues/35491) **fixed**
- [CoreFX: DataContractJsonSerializer.ReadObject throws ArgumentOutOfRangeException](https://github.com/dotnet/corefx/issues/35205)
- [CoreFX: DataContractJsonSerializer.ReadObject throws IndexOutOfRangeException](https://github.com/dotnet/runtime/issues/1410)
- [CoreFX: DataContractSerializer.ReadObject throws ArgumentNullException](https://github.com/dotnet/runtime/issues/1409)
- [CoreFX: Double.Parse throws AccessViolationException on .NET Core 3.0](https://github.com/dotnet/corefx/issues/35780) **fixed**
- [CoreFX: G17 format specifier doesn't always round-trip double values](https://github.com/dotnet/corefx/issues/35369) **fixed**
- [CoreFX: Uri.TryCreate throws IndexOutOfRangeException](https://github.com/dotnet/corefx/issues/35072)
- [CoreFX: XmlReader.Create throws IndexOutOfRangeException](https://github.com/dotnet/corefx/issues/35073) **fixed**
- [DotLiquid: Template.Parse throws ArgumentNullException instead of SyntaxException](https://github.com/dotliquid/dotliquid/issues/333)
- [Esprima .NET: JavaScriptParser.ParseProgram throws ArgumentOutOfRangeException](https://github.com/sebastienros/esprima-dotnet/issues/70) **fixed**
- [Esprima .NET: StackOverflowException when parsing a lot of starting parentheses](https://github.com/sebastienros/esprima-dotnet/issues/104) **fixed**
- [ExcelDataReader: ExcelReaderFactory.CreateBinaryReader can throw unexpected exceptions](https://github.com/ExcelDataReader/ExcelDataReader/issues/383) **fixed**
- [ExcelDataReader: ExcelReaderFactory.CreateBinaryReader throws OutOfMemoryException](https://github.com/ExcelDataReader/ExcelDataReader/issues/382) **fixed**
- [ExCSS: StylesheetParser.Parse throws ArgumentOutOfRangeException](https://github.com/TylerBrinks/ExCSS/issues/101) **fixed**
- [Fluid: FluidTemplate.TryParse and FluidTemplateExtensions.Render throw some unexpected exceptions](https://github.com/sebastienros/fluid/issues/148) **fixed**
- [Fluid: FluidTemplateExtensions.Render hangs permanently](https://github.com/sebastienros/fluid/issues/149) **fixed**
- [Google.Protobuf: MessageParser.ParseFrom throws unexpected exceptions (C#)](https://github.com/protocolbuffers/protobuf/issues/5513) **fixed**
- [GraphQL-Parser: Parser.Parse takes around 18s to parse the 58K file](https://github.com/graphql-dotnet/parser/issues/22) **fixed**
- [GraphQL-Parser: Parser.Parse throws ArgumentOutOfRangeException](https://github.com/graphql-dotnet/parser/issues/21) **fixed**
- [Handlebars.Net: Handlebars.Compile hangs permanently](https://github.com/rexm/Handlebars.Net/issues/283) **fixed**
- [Handlebars.Net: Template engine throws some unexpected exceptions](https://github.com/rexm/Handlebars.Net/issues/282) **fixed**
- [Jil: JSON.DeserializeDynamic throws ArgumentException](https://github.com/kevin-montrose/Jil/issues/316) **fixed**
- [Jint: Engine.Execute can throw many unexpected exceptions](https://github.com/sebastienros/jint/issues/571) **fixed**
- [Jint: Engine.Execute takes more than two minutes to complete (even with the 2s timeout)](https://github.com/sebastienros/jint/issues/586) **fixed**
- [Jint: Engine.Execute throws OutOfMemoryException after 45s (even with the 2s timeout)](https://github.com/sebastienros/jint/issues/587) **fixed**
- [Json.NET: JsonConvert.DeserializeObject can throw several unexpected exceptions](https://github.com/JamesNK/Newtonsoft.Json/issues/1947) **[fixed](https://github.com/JamesNK/Newtonsoft.Json/pull/2922)**
- [Jurassic: ScriptEngine.Execute terminates the process with StackOverflowException](https://github.com/paulbartrum/jurassic/issues/141)
- [Jurassic: ScriptEngine.Execute throws some unexpected exceptions](https://github.com/paulbartrum/jurassic/issues/142) **fixed**
- [Jurassic: ScriptEngine.ExecuteFile hangs permanently instead of throwing JavaScriptException](https://github.com/paulbartrum/jurassic/issues/138) **fixed**
- [Jurassic: ScriptEngine.ExecuteFile throws FormatException](https://github.com/paulbartrum/jurassic/issues/137) **fixed**
- [LumenWorks CSV Reader: CsvReader.ReadNextRecord throws IndexOutOfRangeException](https://github.com/phatcher/CsvReader/issues/67)
- [Markdig: Markdown.ToHtml hangs permanently](https://github.com/lunet-io/markdig/issues/278) **fixed**
- [Markdig: Markdown.ToHtml takes more than two minutes to complete when processing the 32K file](https://github.com/lunet-io/markdig/issues/306) **fixed**
- [Markdig: Markdown.ToHtml throws ArgumentOutOfRangeException](https://github.com/lunet-io/markdig/issues/275) **fixed**
- [Markdig: Markdown.ToHtml throws IndexOutOfRangeException](https://github.com/lunet-io/markdig/issues/276) **fixed**
- [Markdig: Markdown.ToHtml throws IndexOutOfRangeException](https://github.com/lunet-io/markdig/issues/303) **fixed**
- [Markdig: Markdown.ToHtml throws NullReferenceException](https://github.com/lunet-io/markdig/issues/277) **fixed**
- [Markdig: StackOverflowException is throw when converting special markdown to HTML](https://github.com/xoofx/markdig/issues/497) **fixed**
- [MarkdownSharp: Markdown.Transform hangs permanently](https://github.com/StackExchange/MarkdownSharp/issues/8)
- [MessagePack for C#: MessagePackSerializer.Deserialize<dynamic> hangs permanently](https://github.com/neuecc/MessagePack-CSharp/issues/359) **fixed**
- [MessagePack for CLI: Unpacking.UnpackObject throws several unexpected exceptions](https://github.com/msgpack/msgpack-cli/issues/311)
- [Mono.Cecil: ModuleDefinition.ReadModule can throw many (possibly) unexpected exceptions](https://github.com/jbevain/cecil/issues/556)
- [Mono.Cecil: ModuleDefinition.ReadModule hangs permanently](https://github.com/jbevain/cecil/issues/555) **fixed**
- [NCrontab: CrontabSchedule.Parse throws OverflowException instead of CrontabException](https://github.com/atifaziz/NCrontab/issues/43)
- [nHapi: Bad inputs cause unexpected exceptions and permanent hang](https://github.com/nHapiNET/nHapi/issues/196) **fixed**
- [nHapi: Bad inputs cause StackOverflow/Access Violation](https://github.com/nHapiNET/nHapi/issues/198) **fixed**
- [NUglify: Uglify.Js hangs permanently](https://github.com/xoofx/NUglify/issues/63) **fixed**
- [Open XML SDK: Add some security/fuzz testing](https://github.com/OfficeDev/Open-XML-SDK/issues/441)
- [OpenMCDF: OutOfMemoryException when parsing Excel document / endless while-loop](https://github.com/ironfede/openmcdf/issues/30) **fixed**
- [OpenMCDF: System.ArgumentOutOfRangeException take 2](https://github.com/ironfede/openmcdf/issues/39) **fixed**
- [OpenMCDF: System.ArgumentOutOfRangeException when trying to open certain invalid files](https://github.com/ironfede/openmcdf/issues/38) **fixed**
- [OpenMCDF: System.OutOfMemoryException when reading corrupt Word document](https://github.com/ironfede/openmcdf/issues/40) **fixed**
- [PdfPig: StackOverflowException reading corrupt PDF document](https://github.com/UglyToad/PdfPig/issues/33) **fixed**
- [protobuf-net: Serializer.Deserialize can throw many unexpected exceptions](https://github.com/mgravell/protobuf-net/issues/481)
- [protobuf-net: Serializer.Deserialize hangs permanently](https://github.com/mgravell/protobuf-net/issues/479) **fixed**
- [Scriban: Template.ParseLiquid throws ArgumentOutOfRangeException](https://github.com/lunet-io/scriban/issues/121) **fixed**
- [Scriban: Template.ParseLiquid throws NullReferenceException](https://github.com/lunet-io/scriban/issues/120) **fixed**
- [Scriban: Template.Render throws InvalidCastException](https://github.com/lunet-io/scriban/issues/122) **fixed**
- [SharpCompress: Enumerating ZipArchive.Entries collection throws NullReferenceException](https://github.com/adamhathcock/sharpcompress/issues/431)
- [SharpZipLib: ZipInputStream.GetNextEntry hangs permanently](https://github.com/icsharpcode/SharpZipLib/issues/300) **fixed**
- [SixLabors.Fonts: FontDescription.LoadDescription throws ArgumentException](https://github.com/SixLabors/Fonts/issues/96) **fixed**
- [SixLabors.Fonts: FontDescription.LoadDescription throws NullReferenceException](https://github.com/SixLabors/Fonts/issues/97) **fixed**
- [SixLabors.ImageSharp: Handle EOF in Jpeg bit reader when data is bad to prevent DOS attack](https://github.com/SixLabors/ImageSharp/pull/2516) **fixed**
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

AFL works on Linux and macOS. If you are using Windows, you can use any Linux distribution
that works under the [Windows Subsystem for Linux]. For native Windows support, you can use
[libFuzzer](https://github.com/Metalnem/sharpfuzz/blob/master/docs/libFuzzer.md)
instead of AFL.

You will need GNU make and a working compiler
(gcc or clang) in order to compile afl-fuzz.
You will also need to have the [.NET Core 2.1]
or greater installed on your machine in order
to instrument .NET assemblies with SharpFuzz.

To simplify your fuzzing experience, it's also
recommended to install [PowerShell].

[Windows Subsystem for Linux]: https://docs.microsoft.com/en-us/windows/wsl/install-win10
[.NET Core 2.1]: https://dotnet.microsoft.com/download
[PowerShell]: https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell

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

# Install afl-fuzz
sudo make install
cd ..
rm -rf afl-2.52b/

# Install SharpFuzz.CommandLine global .NET tool
dotnet tool install --global SharpFuzz.CommandLine
```

[SharpFuzz.CommandLine]: https://www.nuget.org/packages/SharpFuzz.CommandLine/
[script]: https://github.com/Metalnem/sharpfuzz/raw/master/scripts/install.sh

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

As an example, we are going to fuzz [Jil],
which is a fast JSON serializer and deserializer
(see [SharpFuzz.Samples] for many more examples
of complete fuzzing projects).

**1.** Create a new .NET console project, then add [Jil] and
[SharpFuzz] packages to it by running the following commands:

```shell
dotnet add package Jil
dotnet add package SharpFuzz
```

**2.** In your **Main** function, call **SharpFuzz.Fuzzer.OutOfProcess.Run**
with the function that you want to test as a parameter:

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
      Fuzzer.OutOfProcess.Run(stream =>
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
method. The input data will be provided to us via the
**stream** parameter (if the code you are testing takes
its input as a string, you can use an additional overload
of **Fuzzer.OutOfProcess.Run** that accepts **Action&lt;string&gt;**).

If the code passed to **Fuzzer.OutOfProcess.Run** throws an exception,
it will be reported to afl-fuzz as a crash. However, we
want to treat only *unexpected* exceptions as bugs.
**DeserializationException** is what we expect when
we encounter an invalid JSON input, which is why we
catch it in our example.

**3.** Create a directory with some test cases (one
test is usually more than enough). Test files
should contain some input that is accepted by
your code as valid, and should also be as small as
possible. For example, this is the JSON I'm using
for testing JSON deserializers:

```json
{"menu":{"id":1,"val":"X","pop":{"a":[{"click":"Open()"},{"click":"Close()"}]}}}
```

**4.** Let's say that your project is called ```Fuzzing.csproj```
and that your test cases are in the ```Testcases``` directory.
Start fuzzing by running the [fuzz.ps1] script like this:

```shell
pwsh scripts/fuzz.ps1 Jil.Fuzz.csproj -i Testcases
```

For formats such as HTML, JavaScript, JSON, or SQL,
the fuzzing process can be greatly improved with
the usage of a [dictionary] file. AFL comes with
bunch of dictionaries, which you can find after
installation in ```/usr/local/share/afl/dictionaries/```.
With this in mind, we can improve our fuzzing of Jil like this:

```shell
pwsh scripts/fuzz.ps1 Jil.Fuzz.csproj -i Testcases \
  -x /usr/local/share/afl/dictionaries/json.dict
```

**5.** Sit back and relax! You will often have
some useful results within minutes, but sometimes
it can take more than a day, so be patient.

The input files responsible for unhandled exceptions will
appear in the ```findings/crashes``` directory. The total
number of unique crashes will be displayed in red on the
afl-fuzz status screen.

In practice, the real number of unique exceptions will often
be much lower than the reported number, which is why it's
usually best to write a small program that just goes through
the crashing inputs, runs the fuzzing function on each of
them, and saves only the inputs that produce unique stack traces.

[Jil]: https://www.nuget.org/packages/Jil
[SharpFuzz.Samples]: https://github.com/Metalnem/sharpfuzz-samples
[SharpFuzz]: https://www.nuget.org/packages/SharpFuzz
[dictionary]: https://lcamtuf.blogspot.com/2015/01/afl-fuzz-making-up-grammar-with.html
[fuzz.ps1]: https://github.com/Metalnem/sharpfuzz/raw/master/scripts/fuzz.ps1

## Advanced topics

- [Fuzzing .NET Core](https://github.com/Metalnem/sharpfuzz/blob/master/docs/fuzzing-dotnet-core.md)
- [Out-of-process fuzzing](https://github.com/Metalnem/sharpfuzz/blob/master/docs/miscellaneous.md#out-of-process-fuzzing)
- [Test case minimization](https://github.com/Metalnem/sharpfuzz/blob/master/docs/miscellaneous.md#test-case-minimization)
- [Using libFuzzer with SharpFuzz](https://github.com/Metalnem/sharpfuzz/blob/master/docs/libFuzzer.md)
- [Legacy usage instructions](https://github.com/Metalnem/sharpfuzz/blob/master/docs/legacy-usage-instructions.md)

## Acknowledgements

- **Joe Ranweiler** and the MORSE team - [libFuzzer support on Windows](https://github.com/Metalnem/sharpfuzz/pull/24)
- **Michal Zalewski** - [american fuzzy lop](http://lcamtuf.coredump.cx/afl/)
- **Dmitry Vyukov** - [go-fuzz: randomized testing for Go](https://github.com/dvyukov/go-fuzz)
- **Rody Kersten** - [Kelinci: AFL-based fuzzing for Java](https://github.com/isstac/kelinci)
- **Jb Evain** - [Mono.Cecil](https://github.com/jbevain/cecil)
- **0xd4d** - [dnlib](https://github.com/0xd4d/dnlib)
- **Guido Vranken** - [go-fuzz: libFuzzer support](https://github.com/dvyukov/go-fuzz/pull/217)
