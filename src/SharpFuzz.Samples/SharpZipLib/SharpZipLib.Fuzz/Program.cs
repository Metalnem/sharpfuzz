using System;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using SharpFuzz;

namespace SharpZipLib.Fuzz
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
					using (var zip = new ZipInputStream(file))
					{
						while (zip.GetNextEntry() != null) { }
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (EndOfStreamException) { }
				catch (IndexOutOfRangeException) { }
				catch (NotSupportedException) { }
				catch (ZipException) { }
				catch (StreamDecodingException) { }
				catch (SharpZipBaseException) { }
			});
		}
	}
}
