using iText.Kernel.Pdf;

namespace itext7.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new PdfDocument(new PdfReader(args[0]));
		}
	}
}
