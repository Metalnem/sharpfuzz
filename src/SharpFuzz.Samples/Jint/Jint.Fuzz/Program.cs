using System;
using System.IO;
using Esprima;
using Jint.Runtime;
using SharpFuzz;

namespace Jint.Fuzz
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
					new Engine().Execute(text);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidOperationException) { }
				catch (JavaScriptException) { }
				catch (NullReferenceException) { }
				catch (ParserException) { }
			});
		}
	}
}
