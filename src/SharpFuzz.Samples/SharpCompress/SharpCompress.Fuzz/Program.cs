using System;
using System.IO;
using System.Linq;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpFuzz;

namespace SharpCompress.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var zip = ZipArchive.Open(args[0]))
					{
						zip.Entries.ToList();
					}
				}
				catch (ArchiveException) { }
				catch (ArgumentException) { }
				catch (CryptographicException) { }
				catch (IOException) { }
				catch (NotSupportedException) { }
				catch (NullReferenceException) { }
				catch (OutOfMemoryException) { }
			});
		}
	}
}
