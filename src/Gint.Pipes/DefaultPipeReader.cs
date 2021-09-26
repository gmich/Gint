using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{
    public class DefaultPipeReader : IPipeReader
    {
        private readonly IPipe pipe;

        public DefaultPipeReader(IPipe pipe)
        {
            this.pipe = pipe;
        }

        public PipeReadResult Read(bool advanceCursor = true)
        {
            return pipe.Read();
        }

        public PipeReadResult Read(int offset, int count)
        {
            return pipe.Read(offset, count);
        }

        public Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken, bool advanceCursor = true)
        {
            return pipe.ReadAsync(cancellationToken, advanceCursor);
        }

        public Task<PipeReadResult> ReadAsync(int offset, int count, CancellationToken cancellationToken)
        {
            return pipe.ReadAsync(offset, count, cancellationToken);
        }
    }
}
