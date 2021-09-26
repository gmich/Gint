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

    public sealed class DefaultPipe : IPipe
    {
        private int bytesBuffered = 0;
        private int bytesConsumed = 0;
        private byte[] buffer;
        private object sync = new object();
        private int defaultBufferSize = 2048;
        private readonly PipeOperationState pipeOperationState;

        public DefaultPipe()
        {
            Reader = new DefaultPipeReader(this);
            Writer = new DefaultPipeWriter(this);

            buffer = ArrayPool<byte>.Shared.Rent(defaultBufferSize);
            pipeOperationState = new PipeOperationState();
        }

        public IPipeReader Reader { get; }
        public IPipeWriter Writer { get; }

        private PipeReadResult ReadResult => new PipeReadResult(buffer, pipeOperationState.IsCancelled, pipeOperationState.IsCompleted);

        private int BytesRemaining => buffer.Length - bytesBuffered;

        public void Flush()
        {
            if (pipeOperationState.IsCancelled) return;
            lock (sync)
            {
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

        /// <param name="sizeHint">minimum size</param>
        private void ExtendBuffer(int sizeHint)
        {
            var newSize = Math.Max(buffer.Length * 2, sizeHint);

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

                if (BytesRemaining <= (buffer.Length + bytes.Length))
                {
                    ExtendBuffer(bytes.Length);
                }

                Buffer.BlockCopy(bytes, 0, buffer, bytesBuffered, bytes.Length);
                bytesBuffered += bytes.Length;

                pipeOperationState.EndWrite();
            }
        }

        public PipeReadResult Read()
        {
            lock (sync)
            {
                if (pipeOperationState.IsCompletedOrCancelled)
                {
                    return ReadResult;
                }

                pipeOperationState.BeginRead();

                var bytesToRead = bytesBuffered - bytesConsumed;
                byte[] readBuffer = new byte[bytesToRead];

                Buffer.BlockCopy(buffer, bytesConsumed, readBuffer, 0, bytesToRead);

                pipeOperationState.EndRead();

                return ReadResult;
            }
        }


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

        public Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Flush();
                return Task.FromResult(ReadResult);
            }
            var readRes = Read();
            return Task.FromResult(readRes);
        }

        public Task<PipeReadResult> ReadAsync()
        {
            throw new NotImplementedException();
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

        //No unmanaged resources so no need for the finalizer
        //~DefaultPipe()
        //{
        //    Dispose(false);
        //}

        #endregion
    }
}
