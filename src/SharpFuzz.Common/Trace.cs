namespace SharpFuzz.Common
{
	/// <summary>
	/// Trace contains instrumentation injected into fuzzed programs.
	/// This is internal implementation class not meant for public use.
	/// </summary>
	public static unsafe class Trace
	{
		/// <summary>
		/// Instrumentation bitmap. Contains XORed pairs of data: identifiers of the
		/// currently executing branch and the one that executed immediately before.
		/// </summary>
		public static byte* SharedMem;

		/// <summary>
		/// Identifier of the last executed branch.
		/// </summary>
		public static int PrevLocation;
	}
}
