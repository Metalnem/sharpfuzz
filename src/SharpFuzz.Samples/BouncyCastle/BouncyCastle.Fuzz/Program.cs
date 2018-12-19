using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using SharpFuzz;

namespace BouncyCastle.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					using (var asn = new Asn1InputStream(file))
					{
						while (asn.ReadObject() != null) { }
					}
				}
				catch (ArgumentException) { }
				catch (Asn1Exception) { }
				catch (Asn1ParsingException) { }
				catch (InvalidCastException) { }
				catch (InvalidOperationException) { }
				catch (IOException) { }
			});
		}
	}
}
