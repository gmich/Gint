using System.Threading;
using System.Threading.Tasks;

namespace Gint.Pipes
{
    public interface IPipeReader
    {
        PipeReadResult Read(bool advanceCursor = true);
        PipeReadResult Read(int offset, int count);

        Task<PipeReadResult> ReadAsync(CancellationToken cancellationToken, bool advanceCursor = true);
        Task<PipeReadResult> ReadAsync(int offset, int count, CancellationToken cancellationToken);
    }
}
