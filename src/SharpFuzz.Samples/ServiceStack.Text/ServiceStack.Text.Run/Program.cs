using System.IO;

namespace ServiceStack.Text.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			JsonObject.Parse(text);
		}
	}
}
