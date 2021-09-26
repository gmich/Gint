using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{

    public sealed class GintPipe : IPipe
    {
        private int bytesBuffered = 0;
        private int bytesConsumed = 0;
        private int readCursorHead = 0;

        private byte[] buffer;
        private object sync = new object();
        private readonly PipeOptions pipeOptions;
        private readonly PipeOperationState pipeOperationState;

        public GintPipe() : this(PipeOptions.Default)
        {
        }

        public GintPipe(PipeOptions options)
        {
            pipeOptions = options;
            Reader = new DefaultPipeReader(this);
            Writer = new DefaultPipeWriter(this);

            buffer = ArrayPool<byte>.Shared.Rent(pipeOptions.PreferredBufferSegmentSize);
            pipeOperationState = new PipeOperationState();
        }

        public IPipeReader Reader { get; }
        public IPipeWriter Writer { get; }

        private PipeReadResult ReadResult(byte[] readbuffer) => new PipeReadResult(readbuffer, pipeOperationState.IsCancelled, pipeOperationState.IsCompleted);

        private int BytesRemainingInBuffer => buffer.Length - bytesBuffered;

        public void Flush()
        {
            if (pipeOperationState.IsCancelled) return;
            lock (sync)
            {
                readCursorHead = 0;
                bytesBuffered = 0;
                bytesConsumed = 0;
                ArrayPool<byte>.Shared.Return(buffer);
                buffer = null;
                pipeOperationState.Cancel();
            }
        }

        public void Complete()
        {
            pipeOperationState.Complete();
        }

        /// <param name="sizeHint">minimum size to fit the new bytes</param>
        private void ExtendBuffer(int sizeHint)
        {
            //no need to be based on 8 because ArrayPool takes care of that
            var newSize = Math.Max(buffer.Length + pipeOptions.PreferredBufferSegmentSize, sizeHint);

            var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);

            //return the old buffer to the pool.
            ArrayPool<byte>.Shared.Return(buffer);
            buffer = newBuffer;
        }

        public void Write(byte[] bytes)
        {
            lock (sync)
            {
                pipeOperationState.BeginWrite();

                if (buffer.Length <= (WriteCursor + bytes.Length))
                {
                    ExtendBuffer(bytes.Length);
                }

                Buffer.BlockCopy(bytes, 0, buffer, WriteCursor, bytes.Length);
                bytesBuffered += bytes.Length;

                pipeOperationState.EndWrite();
            }
        }

        public PipeReadResult Read(bool advanceCursor = true)
        {
            lock (sync)
            {
                if (pipeOperationState.IsCancelled)
                {
                    return ReadResult(new byte[0]);
                }

                pipeOperationState.BeginRead();

                var bytesToRead = bytesBuffered - bytesConsumed;
                byte[] readBuffer = new byte[bytesToRead];

                Buffer.BlockCopy(buffer, ReadCursor, readBuffer, 0, bytesToRead);

                bytesConsumed += bytesToRead;

                if (advanceCursor) AdvanceReadCursor(bytesToRead);

                pipeOperationState.EndRead();

                return ReadResult(readBuffer);
            }
        }

        /// <summary>
        /// Move the read cursor forward so the write cursor can re-write the readen blocks and avoid a buffer resize
        /// </summary>
        private void AdvanceReadCursor(int steps)
        {
            readCursorHead += steps;
        }

        /// <summary>
        /// The position where bytes will be writen. Previously writen bytes will be overriden
        /// </summary>
        private int WriteCursor => bytesBuffered - readCursorHead;

        /// <summary>
        /// The position where bytes will be read.
        /// </summary>
        private int ReadCursor => bytesConsumed - readCursorHead;

        public Task WriteAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Flush();
                return Task.FromResult(0);
            }
            Write(bytes);
            return Task.FromResult(0);
        }

        public PipeReadResult Read(int offset, int count)
        {
            lock (sync)
            {
                if (pipeOperationState.IsCancelled)
                {
                    return ReadResult(new byte[0]);
                }
                if ((offset + count) >= buffer.Length)
                {
                    throw new PipeOperationException($"Out of bounds read from offset '{offset}' and count '{count}' for buffer of size {buffer.Length}.");
                }

                pipeOperationState.BeginRead();

                byte[] readBuffer = new byte[count];

                Buffer.BlockCopy(buffer, offset, readBuffer, 0, count);

                pipeOperationState.EndRead();

                return ReadResult(readBuffer);
            }
        }

        public Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken, bool advanceCursor = true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Flush();
            }
            var readRes = Read(advanceCursor);
            return Task.FromResult(readRes);
        }

        public Task<PipeReadResult> ReadAsync(int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Flush();
            }
            var readRes = Read(offset, count);
            return Task.FromResult(readRes);
        }

        #region Dispose

        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                Flush();
            }
            isDisposed = true;
        }

        //No unmanaged resources so no need for a finalizer
        //~DefaultPipe()
        //{
        //    Dispose(false);
        //}

        #endregion
    }
}
