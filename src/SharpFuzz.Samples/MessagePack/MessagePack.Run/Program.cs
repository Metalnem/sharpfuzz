using System.IO;
using MessagePack.Resolvers;

namespace MessagePack.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			{
				MessagePackSerializer.Deserialize<dynamic>(file, ContractlessStandardResolver.Instance);
			}
		}
	}
}
