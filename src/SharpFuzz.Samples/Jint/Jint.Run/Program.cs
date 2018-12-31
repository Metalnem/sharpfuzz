using System;
using System.IO;

namespace Jint.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			new Engine(SetOptions).Execute(text);
		}

		private static void SetOptions(Options options)
		{
			options.LimitRecursion(32).TimeoutInterval(TimeSpan.FromSeconds(2));
		}
	}
}
