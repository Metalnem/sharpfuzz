using System;
using SharpFuzz;

namespace Mono.Cecil.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					ModuleDefinition.ReadModule(args[0], new ReaderParameters(ReadingMode.Immediate));
				}
				catch (AssemblyResolutionException) { }
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
				catch (BadImageFormatException) { }
				catch (IndexOutOfRangeException) { }
				catch (NotImplementedException) { }
				catch (NotSupportedException) { }
				catch (NullReferenceException) { }
				catch (OverflowException) { }
			});
		}
	}
}
