using System.IO;
using Sample;

namespace Google.Protobuf.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				Person.Parser.ParseFrom(file);
			}
		}
	}
}
