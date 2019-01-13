namespace SharpFuzz
{
	// afl-fuzz execution status fault codes (only
	// success and crash are currently being used).
	internal static class Fault
	{
		public const int None = 0;
		public const int Timeout = 1;
		public const int Crash = 2;
	}
}
