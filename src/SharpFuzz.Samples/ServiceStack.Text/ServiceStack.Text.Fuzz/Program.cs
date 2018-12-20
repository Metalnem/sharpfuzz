using System;
using System.IO;
using SharpFuzz;

namespace ServiceStack.Text.Fuzz
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
					JsonObject.Parse(text);
				}
				catch (ArgumentOutOfRangeException) { }
				catch (IndexOutOfRangeException) { }
			});
		}
	}
}
