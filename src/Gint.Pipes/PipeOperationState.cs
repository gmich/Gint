using System;
using System.Runtime.CompilerServices;

namespace Gint.Pipes
{
    internal struct PipeOperationState
    {
        private State state;

        private void GuardNotFinished()
        {
            if ((state & State.Completed) == State.Completed)
            {
                throw new PipeOperationException("Pipe is completed.");
            }
            else if ((state & State.Cancelled) == State.Cancelled)
            {
                throw new PipeOperationException("Pipe is cancelled.");

            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginRead()
        {
            GuardNotFinished();
            if ((state & State.Reading) == State.Reading)
            {
                throw new PipeOperationException("Pipe reading is already in progress.");
            }

            state |= State.Reading;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndRead()
        {
            GuardNotFinished();
            if ((state & State.Reading) != State.Reading)
            {
                throw new PipeOperationException("No pipe reading to complete.");
            }

            state &= ~State.Reading;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginWrite()
        {
            GuardNotFinished();
            if ((state & State.Writing) == State.Writing)
            {
                throw new PipeOperationException("Pipe writing is already in progress.");
            }
            state |= State.Writing;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndWrite()
        {
            GuardNotFinished();

            if ((state & State.Writing) != State.Writing)
            {
                throw new PipeOperationException("No pipe writing to complete.");
            }
            state &= ~State.Writing;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete()
        {
            GuardNotFinished();
            state |= State.Completed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cancel()
        {
            GuardNotFinished();
            state |= State.Cancelled;
        }

        public bool IsWritingActive => (state & State.Writing) == State.Writing;

        public bool IsReadingActive => (state & State.Reading) == State.Reading;

        public bool IsCompleted => (state & State.Completed) == State.Completed;

        public bool IsCancelled => (state & State.Cancelled) == State.Cancelled;

        public bool IsCompletedOrCancelled => IsCompleted || IsCancelled;

        [Flags]
        internal enum State : byte
        {
            Reading = 1,
            Writing = 2,
            Completed = 4,
            Cancelled = 8
        }
    }
}
