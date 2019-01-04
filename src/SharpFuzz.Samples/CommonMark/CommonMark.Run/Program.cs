using System.IO;

namespace CommonMark.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			CommonMarkConverter.Convert(text);
		}
	}
}
