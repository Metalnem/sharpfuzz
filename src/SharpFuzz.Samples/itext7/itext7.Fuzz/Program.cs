using System;
using iText.IO;
using iText.Kernel;
using iText.Kernel.Pdf;
using SharpFuzz;

namespace itext7.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					new PdfDocument(new PdfReader(args[0]));
				}
				catch (FormatException) { }
				catch (InvalidCastException) { }
				catch (IOException) { }
				catch (NullReferenceException) { }
				catch (PdfException) { }
			});
		}
	}
}
