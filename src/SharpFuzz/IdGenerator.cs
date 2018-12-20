using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpFuzz
{
	internal static class IdGenerator
	{
		private const int Retries = 10;

		private static readonly Random random = new Random(0x130f4c29);
		private static readonly byte[] data = new byte[2];
		private static readonly HashSet<int> ids = new HashSet<int>();

		public static int Next()
		{
			var id = MemoryMarshal.Cast<byte, ushort>(data);

			for (int i = 0; i < Retries; ++i)
			{
				random.NextBytes(data);

				if (ids.Add(id[0]))
				{
					break;
				}
			}

			return id[0];
		}
	}
}
