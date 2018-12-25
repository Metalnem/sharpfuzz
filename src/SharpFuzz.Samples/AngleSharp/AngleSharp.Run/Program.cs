using System.IO;
using AngleSharp.Parser.Html;

namespace AngleSharp.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				new HtmlParser().Parse(file);
			}
		}
	}
}
