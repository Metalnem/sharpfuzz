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
					new Engine(SetOptions).Execute(text);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (JavaScriptException) { }
				catch (NullReferenceException) { }
				catch (OverflowException) { }
				catch (ParserException) { }
				catch (RecursionDepthOverflowException) { }
				catch (TimeoutException) { }
			});
		}

		private static void SetOptions(Options options)
		{
			options.LimitRecursion(32).TimeoutInterval(TimeSpan.FromSeconds(2));
		}
	}
}
