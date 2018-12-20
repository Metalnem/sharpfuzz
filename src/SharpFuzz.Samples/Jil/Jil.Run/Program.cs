using System.IO;

namespace Jil.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenText(args[0]))
			{
				JSON.DeserializeDynamic(file);
			}
		}
	}
}
