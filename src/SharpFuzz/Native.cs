using System;
using System.Runtime.InteropServices;

namespace SharpFuzz
{
	// Interop methods for attaching/detaching shared memory segments.
	internal static class Native
	{
		[DllImport("libc", SetLastError = true)]
		public static extern SharedMemoryHandle shmat(int shmid, IntPtr shmaddr, int shmflg);

		[DllImport("libc", SetLastError = true)]
		public static extern int shmdt(IntPtr shmaddr);
	}
}
