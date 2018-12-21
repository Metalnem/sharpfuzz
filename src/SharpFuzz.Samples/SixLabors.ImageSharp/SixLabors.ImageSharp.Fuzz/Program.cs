using System;
using SharpFuzz;

namespace SixLabors.ImageSharp.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					Image.Load(args[0]);
				}
				catch (ImageFormatException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
