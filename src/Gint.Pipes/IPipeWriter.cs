using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{
    public interface IPipeWriter
    {
        void Write(byte[] bytes);
        Task WriteAsync(byte[] bytes, CancellationToken cancellationToken);
    }
}
