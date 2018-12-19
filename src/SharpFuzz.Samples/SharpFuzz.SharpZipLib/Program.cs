using System;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpFuzz.SharpZipLib
{
	public class Program
	{
		/// <see href="https://www.nuget.org/packages/SharpZipLib/">SharpZipLib</see>
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
				catch (SharpZipBaseException) { }
			});
		}
	}
}
