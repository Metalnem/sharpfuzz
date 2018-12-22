using System;
using System.IO;
using System.Text;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharpFuzz;

namespace MongoDB.Bson.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					using (var bson = new BsonBinaryReader(file))
					{
						BsonSerializer.Deserialize(bson, typeof(object));
					}
				}
				catch (ArgumentOutOfRangeException) { }
				catch (DecoderFallbackException) { }
				catch (ArgumentException) { }
				catch (FormatException) { }
				catch (IndexOutOfRangeException) { }
				catch (IOException) { }
				catch (OutOfMemoryException) { }
			});
		}
	}
}
