using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace LumenWorksCsvReader.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenText(args[0]))
			using (var csv = new CsvReader(file, false))
			{
				while (csv.ReadNextRecord()) { }
			}
		}
	}
}
