namespace Mono.Cecil.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ModuleDefinition.ReadModule(args[0], new ReaderParameters(ReadingMode.Immediate));
		}
	}
}
