using System.Linq;
using SharpCompress.Archives.Zip;

namespace SharpCompress.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var zip = ZipArchive.Open(args[0]))
			{
				zip.Entries.ToList();
			}
		}
	}
}
