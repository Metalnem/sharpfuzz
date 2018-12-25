using System.IO;
using System.Text;

namespace ExcelDataReader.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			using (var file = File.OpenRead(args[0]))
			{
				ExcelReaderFactory.CreateBinaryReader(file);
			}
		}
	}
}
