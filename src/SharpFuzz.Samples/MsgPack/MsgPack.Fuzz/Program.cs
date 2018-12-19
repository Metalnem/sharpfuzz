using System;
using System.IO;
using SharpFuzz;

namespace MsgPack.Fuzz
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
						Unpacking.UnpackObject(file);
					}
				}
				catch (InvalidMessagePackStreamException) { }
				catch (MessageNotSupportedException) { }
				catch (MessageTypeException) { }
				catch (OutOfMemoryException) { }
				catch (OverflowException) { }
				catch (UnpackException) { }
			});
		}
	}
}
