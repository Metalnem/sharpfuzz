using System;
using System.IO;
using SharpFuzz;

namespace Jil.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenText(args[0]))
					{
						JSON.DeserializeDynamic(file);
					}
				}
				catch (ArgumentException) { }
				catch (DeserializationException) { }
			});
		}
	}
}
