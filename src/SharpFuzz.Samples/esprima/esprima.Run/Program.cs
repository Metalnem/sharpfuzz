using System.IO;
using Esprima;

namespace esprima.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			var parser = new JavaScriptParser(text);
			parser.ParseProgram();
		}
	}
}
