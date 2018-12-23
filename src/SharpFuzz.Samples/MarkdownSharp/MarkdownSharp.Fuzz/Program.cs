using System.IO;
using SharpFuzz;

namespace MarkdownSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var text = File.ReadAllText(args[0]);
				new Markdown().Transform(text);
			});
		}
	}
}
