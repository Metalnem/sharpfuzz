using System;
using System.IO;
using SharpFuzz;
using SixLabors.Fonts.Exceptions;

namespace SixLabors.Fonts.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					FontDescription.LoadDescription(args[0]);
				}
				catch (ArgumentException) { }
				catch (EndOfStreamException) { }
				catch (InvalidFontFileException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
