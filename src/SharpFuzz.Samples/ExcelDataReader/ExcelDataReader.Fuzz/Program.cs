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
					using (var reader = ExcelReaderFactory.CreateBinaryReader(file))
					{
						do
						{
							while (reader.Read()) { }
						} while (reader.NextResult());
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
