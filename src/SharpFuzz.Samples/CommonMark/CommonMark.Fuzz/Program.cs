using System.IO;
using SharpFuzz;

namespace CommonMark.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
					CommonMarkConverter.Convert(text);
				}
				catch (CommonMarkException) { }
			});
		}
	}
}
