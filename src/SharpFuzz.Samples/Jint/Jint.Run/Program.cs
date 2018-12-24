using System.IO;

namespace Jint.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			new Engine(options => options.LimitRecursion(32)).Execute(text);
		}
	}
}
