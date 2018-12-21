using System.IO;
using SharpFuzz;

namespace NUglify.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var text = File.ReadAllText(args[0]);
				Uglify.Css(text);
			});
		}
	}
}
