## Features

- [ ] Document what to expect on timeout (OOP or server misbehaving)
- [ ] Add my workflow and some tips and tricks
- [ ] Log unique stack traces for each exception
- [ ] Determine why instrumentation varies across runs
- [ ] Verify that instructions after try/catch/finally blocks are instrumented
- [ ] Instrument exception filters
- [ ] Mark the assembly as instrumented
- [ ] Explore persistent mode, resuming, and parallelization
- [ ] Use Jint with timeouts to test Jurassic (also compare other implementations with each other)
- [ ] Fuzz more complex behaviors by using ideas from [go-fuzz-corpus]
- [ ] Use [ILVerify] in CI
- [ ] Create an out-of-process fork server
- [ ] Add error checking to installation script
- [ ] Fuzz Office formats by using [Microsoft Office File Formats Documentation] and extracting the archive

[go-fuzz-corpus]: https://github.com/dvyukov/go-fuzz-corpus
[ILVerify]: https://github.com/dotnet/corert/tree/master/src/ILVerify
[Microsoft Office File Formats Documentation]: https://www.microsoft.com/en-us/download/details.aspx?id=14565

## System

[BigInteger Struct](https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger?view=netcore-2.1)  
[DataContractJsonSerializer.ReadObject Method](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.json.datacontractjsonserializer.readobject?view=netcore-2.1)  
[DateTime.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparse?view=netcore-2.1)  
[DateTimeOffset.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tryparse?view=netcore-2.1)  
[Double.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.double.tryparse?view=netcore-2.1)  
[HttpWebRequest Class](https://docs.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest?view=netcore-2.1)  
[IPAddress.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.tryparse?view=netcore-2.1)  
[Regex Class](https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=netcore-2.1)  
[Serialization in .NET](https://docs.microsoft.com/en-us/dotnet/standard/serialization/)  
[String.Format Method](https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=netcore-2.1)  
[System.CommandLine Namespace](https://github.com/dotnet/command-line-api)  
[System.IO.Compression Namespace](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression?view=netcore-2.1)  
[System.Security.Cryptography Namespace](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography?view=netcore-2.1)  
[System.Security.Cryptography.X509Certificates Namespace](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates?view=netcore-2.1)  
[TimeSpan.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.tryparse?view=netcore-2.1)  
[Uri Class](https://docs.microsoft.com/en-us/dotnet/api/system.uri?view=netcore-2.1)  
[UriParser Class](https://docs.microsoft.com/en-us/dotnet/api/system.uriparser?view=netcore-2.1)  
[Utf8JsonReader Struct](https://apisof.net/catalog/System.Text.Json.Utf8JsonReader)  
[Utf8Parser.TryParse Method](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8parser.tryparse?view=netcore-2.1)  
[XDocument.Load Method](https://docs.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument.load?view=netcore-2.1)  
[XmlDocument.Load Method](https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmldocument.load?view=netcore-2.1)  

## NuGet

[Bond.Core.CSharp](https://www.nuget.org/packages/Bond.Core.CSharp/)  
[CommonMark.NET](https://www.nuget.org/packages/CommonMark.NET/)  
[CsvHelper](https://www.nuget.org/packages/CsvHelper/)  
[dnlib](https://www.nuget.org/packages/dnlib/)  
[DnsClient.NET](https://www.nuget.org/packages/DnsClient/)  
[DotLiquid](https://www.nuget.org/packages/DotLiquid/)  
[esprima](https://www.nuget.org/packages/esprima/)  
[EvoPDF](https://www.nuget.org/packages/evopdf/)  
[FsPickler](https://www.nuget.org/packages/FsPickler/)  
[GemBox.Spreadsheet](https://www.nuget.org/packages/GemBox.Spreadsheet)  
[Iced](https://www.nuget.org/packages/Iced/)  
[ICSharpCode.Decompiler](https://www.nuget.org/packages/ICSharpCode.Decompiler/)  
[libphonenumber-csharp](https://www.nuget.org/packages/libphonenumber-csharp/)  
[Handlebars.Net](https://www.nuget.org/packages/Handlebars.Net/)  
[HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/)  
[HtmlSanitizer](https://www.nuget.org/packages/HtmlSanitizer/)  
[INI Parser](https://www.nuget.org/packages/ini-parser/)  
[Lucene.Net](https://www.nuget.org/packages/Lucene.Net/)  
[Microsoft.Diagnostics.Runtime](https://www.nuget.org/packages/Microsoft.Diagnostics.Runtime/)  
[MimeKit](https://www.nuget.org/packages/MimeKit/)  
[NAudio](https://www.nuget.org/packages/NAudio/)  
[NodaTime](https://www.nuget.org/packages/NodaTime/)  
[NPOI](https://www.nuget.org/packages/NPOI)  
[PDFsharp](https://www.nuget.org/packages/PDFsharp/)  
[Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle/)  
[Scriban](https://www.nuget.org/packages/Scriban/)  
[System.Reflection.Metadata](https://www.nuget.org/packages/System.Reflection.Metadata)  
[System.Reflection.MetadataLoadContext](https://www.nuget.org/packages/System.Reflection.MetadataLoadContext/)  
[UAParser](https://www.nuget.org/packages/UAParser/)  
[Zlib.Portable](https://www.nuget.org/packages/Zlib.Portable/)  

## Other

DotNetty  
JetBrains  
Kestrel  
ML.NET  
Mono  
Razor  
Roslyn  
RyuJIT  
T4  
WPF  
