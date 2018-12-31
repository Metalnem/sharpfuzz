using System;
using System.IO;
using SharpFuzz;

namespace NUglify.Fuzz
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
					Uglify.Js(text);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
