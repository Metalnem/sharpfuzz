using System;
using System.IO;
using SharpFuzz;

namespace Newtonsoft.Json.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
					JsonConvert.DeserializeObject(text);
				}
				catch (ArgumentException) { }
				catch (JsonReaderException) { }
				catch (JsonSerializationException) { }
				catch (JsonWriterException) { }
				catch (NullReferenceException) { }
			});
		}
	}
}
