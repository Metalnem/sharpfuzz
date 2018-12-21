using System;
using System.IO;
using GraphQLParser;
using GraphQLParser.Exceptions;
using SharpFuzz;

namespace GraphQL_Parser.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
					var parser = new Parser(new Lexer());
					parser.Parse(new Source(text));
				}
				catch (ArgumentOutOfRangeException) { }
				catch (GraphQLSyntaxErrorException) { }
			});
		}
	}
}
