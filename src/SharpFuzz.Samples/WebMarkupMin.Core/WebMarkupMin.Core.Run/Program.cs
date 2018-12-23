using System.IO;

namespace WebMarkupMin.Core.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			new HtmlMinifier().Minify(text);
		}
	}
}
