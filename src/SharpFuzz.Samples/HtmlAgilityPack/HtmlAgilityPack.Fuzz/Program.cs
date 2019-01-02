using SharpFuzz;

namespace HtmlAgilityPack.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				new HtmlDocument().Load(args[0]);
			});
		}
	}
}
