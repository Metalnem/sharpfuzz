using System;
using Microsoft.Win32.SafeHandles;

namespace SharpFuzz
{
	internal sealed class SharedMemoryHandle : SafeHandleMinusOneIsInvalid
	{
		public SharedMemoryHandle() : base(true)
		{
		}

		public unsafe Span<byte> Span(int length) => new Span<byte>(handle.ToPointer(), length);
		protected override bool ReleaseHandle() => true;
	}
}
