using System.IO;
using AngleSharp.Parser.Html;
using SharpFuzz;

namespace AngleSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				using (var file = File.OpenRead(args[0]))
				{
					new HtmlParser().Parse(file);
				}
			});
		}
	}
}
