using System;
using System.IO;

namespace SharpFuzz.CommandLine
{
	public class Program
	{
		private const string Usage = @"Usage: sharpfuzz [path-to-assembly]

path-to-assembly:
  The path to an assembly .dll file to instrument.";

		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine(Usage);
				return;
			}

			string path = args[0];

			if (!File.Exists(path))
			{
				Console.Error.WriteLine("Specified file does not exist.");
				return;
			}

			try
			{
				Fuzzer.Instrument(path);
			}
			catch
			{
				Console.Error.WriteLine("Specified file is not a valid .NET assembly.");
				return;
			}
		}
	}
}
