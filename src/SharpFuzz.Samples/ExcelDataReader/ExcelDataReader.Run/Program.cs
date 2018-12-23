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
			using (var reader = ExcelReaderFactory.CreateBinaryReader(file))
			{
				do
				{
					while (reader.Read()) { }
				} while (reader.NextResult());
			}
		}
	}
}
