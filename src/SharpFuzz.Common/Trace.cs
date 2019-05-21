using System;

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

		/// <summary>
		/// Callback that will be called with the unique branch identifier
		/// and the current function name each time some branch is hit. It's
		/// disabled by default due to performance reasons, and will only be
		/// called if the user chose to use it when instrumenting the assembly.
		/// </summary>
		public static Action<int, string> OnBranch;
	}
}
