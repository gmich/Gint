using System;
using System.IO;

namespace Gint.Pipes
{
    public interface IPipe : IPipeReader, IPipeWriter, IDisposable
    {
        IPipeReader Reader { get; }
        IPipeWriter Writer { get; }

        void Flush();
        void Complete();
        Stream AsStream();
        int Length { get; }
    }
}
