using System.IO;

namespace Utf8Json.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				JsonSerializer.Deserialize<dynamic>(file);
			}
		}
	}
}
