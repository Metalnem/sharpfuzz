using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpFuzz
{
	// The stream passed to the fuzzer users should never be closed.
	// This wrapper class forwards all method calls to the underlying
	// stream, but never disposes of it.
	internal sealed class UnclosableStreamWrapper : Stream, IDisposable
	{
		private readonly Stream baseStream;

		public UnclosableStreamWrapper(Stream baseStream)
		{
			this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
		}

		public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
		public override long Length { get => baseStream.Length; }
		public override bool CanWrite { get => baseStream.CanWrite; }
		public override bool CanTimeout { get => baseStream.CanTimeout; }
		public override bool CanSeek { get => baseStream.CanSeek; }
		public override bool CanRead { get => baseStream.CanRead; }
		public override int ReadTimeout { get => baseStream.ReadTimeout; set => baseStream.ReadTimeout = value; }
		public override int WriteTimeout { get => baseStream.WriteTimeout; set => baseStream.WriteTimeout = value; }

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return baseStream.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return baseStream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void Close()
		{
			// This stream can never be closed.
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			return baseStream.CopyToAsync(destination, bufferSize, cancellationToken);
		}

		public override int EndRead(IAsyncResult asyncResult) => baseStream.EndRead(asyncResult);
		public override void EndWrite(IAsyncResult asyncResult) => baseStream.EndWrite(asyncResult);
		public override void Flush() => baseStream.Flush();
		public override Task FlushAsync(CancellationToken cancellationToken) => baseStream.FlushAsync(cancellationToken);
		public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			return baseStream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		public override int ReadByte() => baseStream.ReadByte();
		public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);
		public override void SetLength(long value) => baseStream.SetLength(value);
		public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			return baseStream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		public override void WriteByte(byte value) => baseStream.WriteByte(value);
	}
}
