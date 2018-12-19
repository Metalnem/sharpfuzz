using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SharpFuzz
{
	internal static class IdGenerator
	{
		private const int Retries = 10;

		private static readonly RandomNumberGenerator random = RandomNumberGenerator.Create();
		private static readonly byte[] data = new byte[2];
		private static readonly HashSet<int> ids = new HashSet<int>();

		public static int Next()
		{
			var id = MemoryMarshal.Cast<byte, ushort>(data);

			for (int i = 0; i < Retries; ++i)
			{
				random.GetBytes(data);

				if (ids.Add(id[0]))
				{
					break;
				}
			}

			return id[0];
		}
	}
}
