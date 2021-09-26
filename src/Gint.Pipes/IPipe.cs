using System;

namespace Gint.Pipes
{
    public interface IPipe : IPipeReader, IPipeWriter, IDisposable
    {
        IPipeReader Reader { get; }
        IPipeWriter Writer { get; }

        void Flush();
        void Complete();
    }
}
