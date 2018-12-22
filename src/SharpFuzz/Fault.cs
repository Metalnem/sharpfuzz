namespace SharpFuzz
{
	// afl-fuzz execution status fault codes (only
	// success and crash are currently being used).
	internal enum Fault : byte
	{
		None = 0,
		Timeout = 1,
		Crash = 2
	}
}
