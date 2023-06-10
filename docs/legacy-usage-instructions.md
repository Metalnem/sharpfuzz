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
afl-fuzz -i testcases_dir -o findings_dir -t timeout \
  dotnet path_to_assembly
```

Let's say that our working directory is called ```Fuzzing```.
If it contains the project ```Fuzzing.csproj```, and the
directory called ```Testcases```, the full command might
look like this:

```shell
afl-fuzz -i Testcases -o Findings -t 5000 \
  dotnet bin/Debug/netcoreapp2.1/Fuzzing.dll
```

It's highly recommended that you always specify the timeout
(5000ms from the previous example is a good choice), otherwise
you will often get false crash reports because AFL uses automatic
timeout calculation, which is too sensitive and unsuitable for
managed languages.

For formats such as HTML, JavaScript, JSON, or SQL,
the fuzzing process can be greatly improved with
the usage of a [dictionary] file. AFL comes with
bunch of dictionaries, which you can find after
installation in ```/usr/local/share/afl/dictionaries/```.
With this in mind, we can improve our fuzzing of Jil like this:

```shell
afl-fuzz -i Testcases -o Findings -t 5000 \
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
afl-fuzz -i testcases_dir -o findings_dir -t 5000 -m 10000 \
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
