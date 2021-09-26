namespace Gint.Pipes
{
    public struct PipeReadResult
    {
        public PipeReadResult(byte[] buffer, bool isCanceled, bool isCompleted)
        {
            Buffer = buffer;
            IsCanceled = isCanceled;
            IsCompleted = isCompleted;
        }

        public byte[] Buffer { get; }
        public bool IsCanceled { get; }
        public bool IsCompleted { get; }
    }
}
