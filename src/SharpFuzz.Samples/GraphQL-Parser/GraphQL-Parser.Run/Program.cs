using System.IO;
using GraphQLParser;

namespace GraphQL_Parser.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			var parser = new Parser(new Lexer());
			parser.Parse(new Source(text));
		}
	}
}
