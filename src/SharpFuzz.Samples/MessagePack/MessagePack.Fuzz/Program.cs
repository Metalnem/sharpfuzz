using System;
using System.IO;
using MessagePack.Resolvers;
using SharpFuzz;

namespace MessagePack.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					{
						MessagePackSerializer.Deserialize<dynamic>(file, ContractlessStandardResolver.Instance);
					}
				}
				catch (ArgumentException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidOperationException) { }
				catch (OutOfMemoryException) { }
				catch (OverflowException) { }
			});
		}
	}
}
