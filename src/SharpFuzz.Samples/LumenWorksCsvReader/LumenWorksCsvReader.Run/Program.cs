using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace LumenWorksCsvReader.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenText("/Users/Metalnem/Temp/findings/crashes/id:000000,sig:02,src:000000,op:havoc,rep:64"))
			using (var csv = new CsvReader(file, false))
			{
				while (csv.ReadNextRecord()) { }
			}
		}
	}
}
