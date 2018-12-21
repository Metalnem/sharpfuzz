using System.IO;
using YamlDotNet.RepresentationModel;

namespace YamlDotNet.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenText(args[0]))
			{
				new YamlStream().Load(file);
			}
		}
	}
}
