using System;
using System.IO;
using System.Text;
using ExcelDataReader.Exceptions;
using SharpFuzz;

namespace ExcelDataReader.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					{
						ExcelReaderFactory.CreateBinaryReader(file);
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (ExcelReaderException) { }
				catch (FormatException) { }
				catch (InvalidOperationException) { }
				catch (OutOfMemoryException) { }
			});
		}
	}
}
