using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpZipLib.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			using (var zip = new ZipInputStream(file))
			{
				while (zip.GetNextEntry() != null) { }
			}
		}
	}
}
