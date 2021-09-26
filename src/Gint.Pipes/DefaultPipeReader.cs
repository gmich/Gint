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

        public PipeReadResult Read()
        {
            return pipe.Read();
        }

        public Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken)
        {
            return pipe.ReadAsync(cancellationToken);

        }
    }
}
