using System;
using System.IO;
using SharpFuzz;

namespace Utf8Json.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					{
						JsonSerializer.Deserialize<dynamic>(file);
					}
				}
				catch (ArgumentNullException) { }
				catch (ArgumentException) { }
				catch (FormatException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidOperationException) { }
				catch (JsonParsingException) { }
			});
		}
	}
}
