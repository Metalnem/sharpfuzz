using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			SpreadsheetDocument.Open(args[0], false);
		}
	}
}
