using System.IO;

namespace ExCSS.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				var parser = new StylesheetParser();
				parser.Parse(file);
			}
		}
	}
}
