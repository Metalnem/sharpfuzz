# Fuzzing .NET Core

.NET Core runtime is made of mixed-mode assemblies, which is
why we can't directly instrument them. There are two different
strategies for fuzzing them, depending on whether the class
you want to instrument is part of the **System.Private.CoreLib**
or not.

## Fuzzing classes outside the System.Private.CoreLib

As an example, we are going to fuzz the [XmlReader] class.
The complete fuzzing project from this tutorial can be found
in the [CoreFX] directory, which is a part of the [SharpFuzz
samples] repository.

**1.** As always, the first step is to create the fuzzing
project, and then write the fuzzing function. Here is how
it might look in the case of **XmlReader**:

```csharp
public static void Main(string[] args)
{
  Fuzzer.Run(stream =>
  {
    try
    {
      using (var xml = XmlReader.Create(stream))
      {
        while (xml.Read()) { }
      }
    }
    catch (XmlException) { }
  });
}
```

**2.** As I have said earlier, the official .NET Core runtime
package contains mixed-mode assemblies, but the assemblies
uploaded to dotnet-blob feed are IL-only. The full details of using
the .NET Core assemblies from dotnet-blob feed can be found in [this]
document. In short, you have to do three things. The first step
is to create the ```NuGet.Config``` file in your project directory
with the following contents:

```xml
<configuration>
  <packageSources>
    <add key="dotnetcore-feed" value="https://dotnetfeed.blob.core.windows.net/dotnet-core/index.json" />
  </packageSources>
</configuration>
```

The second step is to configure your project to use
the latest version of the **Microsoft.Private.CoreFx.NETCoreApp**
package:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Private.CoreFx.NETCoreApp" Version="4.6.0-*" />
</ItemGroup>
```

**Microsoft.Private.CoreFx.NETCoreApp** package conflicts with the
normal **Microsoft.NETCore.App** package, which is why the third
step is to add the following element to your project file (if
you are using macOS, replace **linux-x64** with **osx-x64**):

```xml
<PropertyGroup>
  <PackageConflictPreferredPackages>Microsoft.Private.CoreFx.NETCoreApp;runtime.linux-x64.Microsoft.Private.CoreFx.NETCoreApp;$(PackageConflictPreferredPackages)</PackageConflictPreferredPackages>
</PropertyGroup>
```

**3.** Publish your project as a self-contained application. In the
case of Linux, you can do it like this:

```shell
dotnet publish -r linux-x64
```

In the case of macOS, replace **linux-x64** with **osx-x64**. Your
application (and all .NET Core assemblies) will be located in the
```bin/Debug/netcoreapp3.0/linux-x64/publish``` directory (debug
configuration is default, but you can also publish your application
in release mode).

**4.** Now you can instrument the assembly you want. If you don't
know which assembly contains the type you want to fuzz, you can find
that out by inspecting its **Assembly.CodeBase** property. In our case,
```typeof(XmlReader).Assembly.CodeBase``` says that the **XmlReader**
is located in the ```System.Private.Xml.dll```. You can now instrument
it like this:

```shell
sharpfuzz bin/Debug/netcoreapp3.0/linux-x64/publish/System.Private.Xml.dll
```

**5.** You are now ready to start the fuzzing. Because the
published application is now self-contained, you don't have
to use the ```dotnet``` command, so running afl-fuzz should
look something like this:

```shell
afl-fuzz -i Testcases -o Findings \
  bin/Debug/netcoreapp3.0/linux-x64/publish/project_name
```

[XmlReader]: https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=netcore-3.0
[CoreFX]: https://github.com/Metalnem/sharpfuzz-samples/tree/master/CoreFX
[SharpFuzz samples]: https://github.com/Metalnem/sharpfuzz-samples
[this]: https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/dogfooding.md

## Fuzzing classes inside the System.Private.CoreLib

As an example, we are going to fuzz the [DateTime] struct.
The complete fuzzing project from this tutorial can be found
in the [System.Private.CoreLib2] directory, which is a part of
the [SharpFuzz samples] repository.

**1.** Again, we have to create the fuzzing project, and then
write the fuzzing function:

```csharp
public static void Main(string[] args)
{
  Fuzzer.Run(text =>
  {
    if (DateTime.TryParse(text, out var dt1))
    {
      var s = dt1.ToString("O");
      var dt2 = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);

      if (dt1 != dt2)
      {
        throw new Exception();
      }
    }
  });
}
```

**2.** Build the [CoreCLR] repository. If you are using .NET Core 2.2,
you should build the **release/2.2** branch. In the case of .NET Core
3.0, you can just build the master. The important thing is to create
the IL-only build, which you can do like this:

```shell
./build.sh skiptests skipcrossgen skipnative release
```

As the result of this operation, you will have the IL-only version
of the ```System.Private.CoreLib.dll``` assembly in the
```bin/Product/Linux.x64.Release``` (or ```bin/Product/OSX.x64.Release```).

**3.** Publish your project as a self-contained application,
and then copy the ```System.Private.CoreLib.dll``` to the
resulting ```publish``` directory.

**4.** Instrument the class you want. In contrast to fuzzing
ordinary assemblies, you can't just instrument all the classes.
The most important reason for this is that fuzzing all the
classes would lead the fuzzer to explore many irrelevant paths.
Instead of instrumenting the whole assembly, you will have to
select the list of classes/namespaces (or just any prefixes
of the full class names located in the assembly) like this:

```shell
sharpfuzz bin/Debug/netcoreapp2.2/linux-x64/publish/System.Private.CoreLib.dll System.DateTime System.Globalization.DateTime
```

The tricky part is to select all the relevant classes.
Unfortunately, you will have to do this manually, which
requires some familiarity with the CoreCLR.

**5.** You are now ready to start the fuzzing. As in the previous
example, you can run afl-fuzz with the following command:

```shell
afl-fuzz -i Testcases -o Findings \
  bin/Debug/netcoreapp2.2/linux-x64/publish/project_name
```

[DateTime]: https://docs.microsoft.com/en-us/dotnet/api/system.datetime?view=netcore-2.2
[System.Private.CoreLib2]: https://github.com/Metalnem/sharpfuzz-samples/tree/master/System.Private.CoreLib2
[SharpFuzz samples]: https://github.com/Metalnem/sharpfuzz-samples
[CoreCLR]: https://github.com/dotnet/coreclr
