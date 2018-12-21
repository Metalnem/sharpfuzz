using System;
using System.IO;
using ProtoBuf;
using SharpFuzz;

namespace protobuf_net.Fuzz
{
	[ProtoContract]
	public class Person
	{
		[ProtoMember(1)] public int Id { get; set; }
		[ProtoMember(2)] public string Name { get; set; }
		[ProtoMember(3)] public Address Address { get; set; }
	}

	[ProtoContract]
	public class Address
	{
		[ProtoMember(1)] public string Line1 { get; set; }
		[ProtoMember(2)] public string Line2 { get; set; }
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					using (var file = File.OpenRead(args[0]))
					{
						Serializer.Deserialize<Person>(file);
					}
				}
				catch (ArgumentException) { }
				catch (IndexOutOfRangeException) { }
				catch (InvalidOperationException) { }
				catch (IOException) { }
				catch (OutOfMemoryException) { }
				catch (OverflowException) { }
				catch (ProtoException) { }
			});
		}
	}
}
