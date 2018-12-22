using System;
using System.IO;
using SharpFuzz;

namespace ExCSS.Fuzz
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
						var parser = new StylesheetParser();
						parser.Parse(file);
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ParseException) { }
			});
		}
	}
}
