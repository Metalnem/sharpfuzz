using System;
using System.IO;
using Jint;
using Jint.Runtime;
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
					var text = File.ReadAllText(args[0]);

					if (RunJint(text))
					{
						var engine = new ScriptEngine();
						engine.Execute(text);
					}
				}
				catch (JavaScriptException) { }
			});
		}

		private static bool RunJint(string code)
		{
			try { new Engine(SetOptions).Execute(code); }
			catch (RecursionDepthOverflowException) { return false; }
			catch (TimeoutException) { return false; }
			catch (Exception) { return true; }

			return true;
		}

		private static void SetOptions(Options options)
		{
			options.LimitRecursion(16).TimeoutInterval(TimeSpan.FromSeconds(1));
		}
	}
}
