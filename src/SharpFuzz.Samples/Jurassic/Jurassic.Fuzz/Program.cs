using System;
using SharpFuzz;

namespace Jurassic.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					var engine = new ScriptEngine();
					engine.ExecuteFile(args[0]);
				}
				catch (FormatException) { }
				catch (JavaScriptException) { }
			});
		}
	}
}
