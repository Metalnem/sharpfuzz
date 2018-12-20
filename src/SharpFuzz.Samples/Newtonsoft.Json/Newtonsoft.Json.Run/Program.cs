using System.IO;

namespace Newtonsoft.Json.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			JsonConvert.DeserializeObject(text);
		}
	}
}
