using System;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using SharpFuzz;

namespace LumenWorksCsvReader.Fuzz
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
					using (var csv = new CsvReader(file, false))
					{
						while (csv.ReadNextRecord()) { }
					}
				}
				catch (IndexOutOfRangeException) { }
				catch (MalformedCsvException) { }
			});
		}
	}
}
