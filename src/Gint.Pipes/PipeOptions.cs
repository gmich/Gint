namespace Gint.Pipes
{
    public class PipeOptions
    {
        /// <summary>
        /// Should be 8^x since it's for a byte array
        /// </summary>
        public int PreferredBufferSegmentSize { get; init; }

        public static PipeOptions Default => new PipeOptions
        {
            PreferredBufferSegmentSize = 256
        };
    }
}
