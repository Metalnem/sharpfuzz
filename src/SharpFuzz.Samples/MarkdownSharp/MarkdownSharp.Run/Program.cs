using System.IO;

namespace MarkdownSharp.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			new Markdown().Transform(text);
		}
	}
}
