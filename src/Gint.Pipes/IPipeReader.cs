using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{
    public interface IPipeReader
    {
        PipeReadResult Read();
        Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken);
    }
}
