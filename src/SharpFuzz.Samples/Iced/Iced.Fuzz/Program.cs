using System.IO;
using Iced.Intel;
using SharpFuzz;

namespace Iced.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var code = File.ReadAllBytes(args[0]);
				var reader = new ByteArrayCodeReader(code);

				var decoder = Decoder.Create(64, reader);
				decoder.InstructionPointer = 0x00007FFAC46ACDA4;

				var end = decoder.InstructionPointer + (uint)code.Length;
				var instructions = new InstructionList();

				while (decoder.InstructionPointer < end)
				{
					decoder.Decode(out instructions.AllocUninitializedElement());
				}
			});
		}
	}
}
