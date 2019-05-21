using System;
using System.IO;
using System.Linq;

namespace SharpFuzz.CommandLine
{
	public class Program
	{
		private const string Usage = @"Usage: sharpfuzz [path-to-assembly] [prefix ...]

path-to-assembly:
  The path to an assembly .dll file to instrument.

prefix:
  The class or the namespace to instrument.
  If not present, all types in the assembly will be instrumented.
  At least one prefix is required when instrumenting System.Private.CoreLib.
  
Examples:
  sharpfuzz Newtonsoft.Json.dll
  sharpfuzz System.Private.CoreLib.dll System.Number
  sharpfuzz System.Private.CoreLib.dll System.DateTimeFormat System.DateTimeParse";

		public static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine(Usage);
				return 0;
			}

			string path = args[0];

			if (!File.Exists(path))
			{
				Console.Error.WriteLine("Specified file does not exist.");
				return 1;
			}

			var isCoreLib = Path.GetFileNameWithoutExtension(path) == "System.Private.CoreLib";
			var prefixes = args.Skip(1).ToList();

			if (isCoreLib && prefixes.Count == 0)
			{
				Console.Error.WriteLine("At least one prefix is required when instrumenting System.Private.CoreLib.");
				return 1;
			}

			try
			{
				var enableOnBranchCallback = Environment.GetEnvironmentVariable("SHARPFUZZ_ENABLE_ON_BRANCH_CALLBACK") is object;
				var types = Fuzzer.Instrument(path, Matcher, enableOnBranchCallback);

				if (Environment.GetEnvironmentVariable("SHARPFUZZ_PRINT_INSTRUMENTED_TYPES") is object)
				{
					foreach (var type in types)
					{
						Console.WriteLine(type);
					}
				}
			}
			catch (InstrumentationException ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			catch
			{
				Console.Error.WriteLine("Failed to instrument the specified file, most likely because it's not a valid .NET assembly.");
				return 1;
			}

			bool Matcher(string type)
			{
				return prefixes.Count == 0 || prefixes.Any(prefix => type.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
			}

			return 0;
		}
	}
}
