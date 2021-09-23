//https://github.com/dotnet/corefx/tree/master/src/System.IO.Pipelines/src/System/IO/Pipelines

using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Gint.Pipes
{
    public class Pipe
    {
        public MemoryPool<string> pool;

        public Pipe()
        {
            pool = MemoryPool<string>.Shared;

        }
        private const int DefaultMinimumSegmentSize = 4096;
        public void Allocate(int sizeHint)
        {
            pool.Rent(GetSegmentSize(sizeHint, pool.MaxBufferSize));
        }

        private int GetSegmentSize(int sizeHint, int maxBufferSize = int.MaxValue)
        {
            // First we need to handle case where hint is smaller than minimum segment size
            sizeHint = Math.Max(DefaultMinimumSegmentSize, sizeHint);
            // After that adjust it to fit into pools max buffer size
            var adjustedToMaximumSize = Math.Min(maxBufferSize, sizeHint);
            return adjustedToMaximumSize;
        }
    }

    internal struct BufferSegmentStack
    {
        private SegmentAsValueType[] _array;
        private int _size;

        public BufferSegmentStack(int size)
        {
            _array = new SegmentAsValueType[size];
            _size = 0;
        }

        public int Count => _size;

        public bool TryPop([NotNullWhen(true)] out BufferSegment? result)
        {
            int size = _size - 1;
            SegmentAsValueType[] array = _array;

            if ((uint)size >= (uint)array.Length)
            {
                result = default;
                return false;
            }

            _size = size;
            result = array[size];
            array[size] = default;
            return true;
        }

        // Pushes an item to the top of the stack.
        public void Push(BufferSegment item)
        {
            int size = _size;
            SegmentAsValueType[] array = _array;

            if ((uint)size < (uint)array.Length)
            {
                array[size] = item;
                _size = size + 1;
            }
            else
            {
                PushWithResize(item);
            }
        }

        // Non-inline from Stack.Push to improve its code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void PushWithResize(BufferSegment item)
        {
            Array.Resize(ref _array, 2 * _array.Length);
            _array[_size] = item;
            _size++;
        }

        /// <summary>
        /// A simple struct we wrap reference types inside when storing in arrays to
        /// bypass the CLR's covariant checks when writing to arrays.
        /// </summary>
        /// <remarks>
        /// We use <see cref="SegmentAsValueType"/> as a wrapper to avoid paying the cost of covariant checks whenever
        /// the underlying array that the <see cref="BufferSegmentStack"/> class uses is written to.
        /// We've recognized this as a perf win in ETL traces for these stack frames:
        /// clr!JIT_Stelem_Ref
        ///   clr!ArrayStoreCheck
        ///     clr!ObjIsInstanceOf
        /// </remarks>
        private readonly struct SegmentAsValueType
        {
            private readonly BufferSegment _value;
            private SegmentAsValueType(BufferSegment value) => _value = value;
            public static implicit operator SegmentAsValueType(BufferSegment s) => new SegmentAsValueType(s);
            public static implicit operator BufferSegment(SegmentAsValueType s) => s._value;
        }
    }

    internal sealed class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        private object? _memoryOwner;
        private BufferSegment? _next;
        private int _end;

        /// <summary>
        /// The End represents the offset into AvailableMemory where the range of "active" bytes ends. At the point when the block is leased
        /// the End is guaranteed to be equal to Start. The value of Start may be assigned anywhere between 0 and
        /// Buffer.Length, and must be equal to or less than End.
        /// </summary>
        public int End
        {
            get => _end;
            set
            {
                Debug.Assert(value <= AvailableMemory.Length);

                _end = value;
                Memory = AvailableMemory.Slice(0, value);
            }
        }

        /// <summary>
        /// Reference to the next block of data when the overall "active" bytes spans multiple blocks. At the point when the block is
        /// leased Next is guaranteed to be null. Start, End, and Next are used together in order to create a linked-list of discontiguous
        /// working memory. The "active" memory is grown when bytes are copied in, End is increased, and Next is assigned. The "active"
        /// memory is shrunk when bytes are consumed, Start is increased, and blocks are returned to the pool.
        /// </summary>
        public BufferSegment? NextSegment
        {
            get => _next;
            set
            {
                Next = value;
                _next = value;
            }
        }

        public void SetOwnedMemory(IMemoryOwner<byte> memoryOwner)
        {
            _memoryOwner = memoryOwner;
            AvailableMemory = memoryOwner.Memory;
        }

        public void SetOwnedMemory(byte[] arrayPoolBuffer)
        {
            _memoryOwner = arrayPoolBuffer;
            AvailableMemory = arrayPoolBuffer;
        }

        public void ResetMemory()
        {
            if (_memoryOwner is IMemoryOwner<byte> owner)
            {
                owner.Dispose();
            }
            else
            {
                Debug.Assert(_memoryOwner is byte[]);
                byte[] poolArray = (byte[])_memoryOwner;
                ArrayPool<byte>.Shared.Return(poolArray);
            }

            // Order of below field clears is significant as it clears in a sequential order
            // https://github.com/dotnet/corefx/pull/35256#issuecomment-462800477
            Next = null;
            RunningIndex = 0;
            Memory = default;
            _memoryOwner = null;
            _next = null;
            _end = 0;
            AvailableMemory = default;
        }

        // Exposed for testing
        internal object? MemoryOwner => _memoryOwner;

        public Memory<byte> AvailableMemory { get; private set; }

        public int Length => End;

        public int WritableBytes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AvailableMemory.Length - End;
        }

        public void SetNext(BufferSegment segment)
        {
            Debug.Assert(segment != null);
            Debug.Assert(Next == null);

            NextSegment = segment;

            segment = this;

            while (segment.Next != null)
            {
                Debug.Assert(segment.NextSegment != null);
                segment.NextSegment.RunningIndex = segment.RunningIndex + segment.Length;
                segment = segment.NextSegment;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetLength(BufferSegment startSegment, int startIndex, BufferSegment endSegment, int endIndex)
        {
            return (endSegment.RunningIndex + (uint)endIndex) - (startSegment.RunningIndex + (uint)startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetLength(long startPosition, BufferSegment endSegment, int endIndex)
        {
            return (endSegment.RunningIndex + (uint)endIndex) - startPosition;
        }

    }

    public class PipeWriter
    {

    }

    public class PipeReader
    {

    }

    public class Buffer
    {

    }

}
