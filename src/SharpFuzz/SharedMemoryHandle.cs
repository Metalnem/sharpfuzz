using Microsoft.Win32.SafeHandles;

namespace SharpFuzz
{
	// Represents a wrapper class for a shared memory handle,
	// obtained by attaching the shared memory segment to the address
	// space of the calling process using the shmat system call.
	internal sealed class SharedMemoryHandle : SafeHandleMinusOneIsInvalid
	{
		public SharedMemoryHandle() : base(true) { }
		protected override bool ReleaseHandle() => Native.shmdt(handle) == 0;
	}
}
