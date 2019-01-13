using System;

namespace SharpFuzz
{
	/// <summary>
	/// Represents errors that occur during assembly instrumentation.
	/// </summary>
	public sealed class InstrumentationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InstrumentationException"/>
		/// class with a specified error message.
		/// </summary>
		/// <param name="message">Message that describes the error.</param>
		public InstrumentationException(string message) : base(message) { }
	}
}
