using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoDB.Bson.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var file = File.OpenRead(args[0]))
			using (var bson = new BsonBinaryReader(file))
			{
				BsonSerializer.Deserialize(bson, typeof(object));
			}
		}
	}
}
