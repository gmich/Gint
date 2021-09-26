using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{
    public class DefaultPipeWriter : IPipeWriter
    {
        private readonly IPipe pipe;

        public DefaultPipeWriter(IPipe pipe)
        {
            this.pipe = pipe;
        }
        public void Write(byte[] bytes)
        {
            pipe.Write(bytes);
        }

        public Task WriteAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            return pipe.WriteAsync(bytes, cancellationToken);
        }
    }
}
