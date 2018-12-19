using System.IO;
using Org.BouncyCastle.Asn1;

namespace BouncyCastle.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			using (var asn = new Asn1InputStream(file))
			{
				while (asn.ReadObject() != null) { }
			}
		}
	}
}
