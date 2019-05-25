using System;
using System.Linq;
using System.Linq.Expressions;

namespace SharpFuzz
{
	internal sealed class TraceWrapper
	{
		private readonly Action resetPrevLocation;

		// When instrumenting types in the System.Private.CoreLib.dll assembly we
		// have to embed the Trace class (we can't reference any external assembly).
		// That's why we have to use reflection to initialize the shared memory,
		// and also to reset the previous location on each fuzzing iteration.
		public unsafe TraceWrapper(byte* sharedMem)
		{
			Common.Trace.SharedMem = sharedMem;

			var types = typeof(object).Assembly.GetTypes();
			var traceType = types.FirstOrDefault(t => t.FullName == typeof(Common.Trace).FullName);

			if (traceType != null)
			{
				var sharedMemField = traceType.GetField(nameof(Common.Trace.SharedMem));
				sharedMemField.SetValue(null, System.Reflection.Pointer.Box(sharedMem, typeof(byte*)));

				// Compiling the PrevLocation = 0 assignment so we don't
				// have to use the reflection on each fuzzer iteration.
				var prevLocationField = traceType.GetField(nameof(Common.Trace.PrevLocation));

				var body = Expression.Assign(
					Expression.Field(null, prevLocationField),
					Expression.Constant(0)
				);

				resetPrevLocation = Expression.Lambda<Action>(body).Compile();
			}
		}

		public void ResetPrevLocation()
		{
			Common.Trace.PrevLocation = 0;
			resetPrevLocation?.Invoke();
		}
	}
}
