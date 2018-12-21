using System.IO;

namespace NUglify.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			Uglify.Js(text);
		}
	}
}
