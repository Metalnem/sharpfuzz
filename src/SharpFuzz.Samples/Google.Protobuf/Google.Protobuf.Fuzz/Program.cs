using System;
using System.IO;
using Sample;
using SharpFuzz;

namespace Google.Protobuf.Fuzz
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
						Person.Parser.ParseFrom(file);
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ArgumentException) { }
				catch (InvalidOperationException) { }
				catch (InvalidProtocolBufferException) { }
				catch (OutOfMemoryException) { }
			});
		}
	}
}
