# Fuzzing .NET Core

.NET Core runtime is made of mixed-mode assemblies, which is
why we can't directly instrument them. There are two different
strategies for fuzzing them, depending on whether the class
you want to instrument is part of the **System.Private.CoreLib**
or not.

## Fuzzing classes outside the System.Private.CoreLib

As an example, we are going to fuzz the [XmlReader] class.
The complete fuzzing project from this tutorial can be found
in the [CoreFX2] directory, which is a part of the [SharpFuzz
samples] repository.

**1.** As always, the first step is to create the fuzzing
project, and then write the fuzzing function. Here is how
it might look in the case of **XmlReader**:

```csharp
public static void Main(string[] args)
{
  Fuzzer.Run(() =>
  {
    try
    {
      using (var stream = File.OpenRead(args[0]))
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
uploaded to [MyGet feed] are IL-only. The full details of using
the .NET Core assemblies from MyGet feed can be found in [this]
document. In short, you have to do three things. The first step
is to create the ```NuGet.Config``` file in your project directory
with the following contents:

```xml
<configuration>
  <packageSources>
    <add key="dotnet-core" value="https://dotnet.myget.org/F/dotnet-core/api/v3/index.json"/>
  </packageSources>
</configuration>
```

The second step is to configure your project to use the
**Microsoft.Private.CoreFx.NETCoreApp** package:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Private.CoreFx.NETCoreApp" Version="4.5.220-servicing-27414-05" />
</ItemGroup>
```

If you are fuzzing .NET Core 2.2, you should select the latest
servicing version of the package. If you are fuzzing .NET Core
3.0, you can just select the latest available package.

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
```bin/Debug/netcoreapp2.2/linux-x64/publish``` directory (debug
configuration is default, but you can also publish your application
in release mode).

**4.** Now you can instrument the assembly you want. If you don't
know which assembly contains the type you want to fuzz, you can find
that out by inspecting its **Assembly.CodeBase** property. In our case,
```typeof(XmlReader).Assembly.CodeBase``` says that the **XmlReader**
is located in the ```System.Private.Xml.dll```. You can now instrument
it like this:

```shell
sharpfuzz bin/Debug/netcoreapp2.2/linux-x64/publish/System.Private.Xml.dll
```

**5.** You are now ready to start the fuzzing. Because the
published application is now self-contained, you don't have
to use the ```dotnet``` command, so running afl-fuzz should
look something like this:

```shell
afl-fuzz -i Testcases -o Findings \
  bin/Debug/netcoreapp2.2/linux-x64/publish/project_name @@
```

[XmlReader]: https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=netcore-2.2
[CoreFX2]: https://github.com/Metalnem/sharpfuzz-samples/tree/master/CoreFX2
[SharpFuzz samples]: https://github.com/Metalnem/sharpfuzz-samples
[MyGet feed]: https://dotnet.myget.org/feed/dotnet-core/package/nuget/Microsoft.Private.CoreFx.NETCoreApp
[this]: https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/dogfooding.md
