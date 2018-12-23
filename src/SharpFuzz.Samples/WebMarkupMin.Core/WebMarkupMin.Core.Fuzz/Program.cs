using System.IO;
using SharpFuzz;

namespace WebMarkupMin.Core.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var text = File.ReadAllText(args[0]);
				new HtmlMinifier().Minify(text);
			});
		}
	}
}
