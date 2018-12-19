using System;

namespace SharpFuzz.Common
{
	public static class Trace
	{
		public static readonly byte[] SharedMem = new byte[65536];
		public static int PrevLocation = 0;
	}
}
