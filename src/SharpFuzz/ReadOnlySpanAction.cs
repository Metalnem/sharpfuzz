using System;

namespace SharpFuzz
{
	/// <summary>
	/// Encapsulates a method that has a single parameter and does not return a value.
	/// </summary>
	/// <param name="span">
	/// The parameter of the method that this delegate encapsulates.
	/// </param>
	public delegate void ReadOnlySpanAction(ReadOnlySpan<byte> span);
}
