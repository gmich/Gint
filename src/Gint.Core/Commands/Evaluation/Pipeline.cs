using Gint.Pipes;

namespace Gint
{
    internal class Pipeline
    {
        public CommandScope PreviousScope { get; init; }
        public IPipe PreviousPipe { get; init; }
        public CommandScope Scope { get; init; }
        public IPipe Pipe { get; init; }
    }
}
