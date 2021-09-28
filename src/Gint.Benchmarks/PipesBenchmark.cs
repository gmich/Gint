using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Gint.Markup;
using Gint.Pipes;
using System.IO;

namespace Gint.Benchmarks
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class PipesBenchmark
    {
        private readonly byte[] buffer = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit".ToUTF8EncodedByteArray();

        [Benchmark]
        public byte[] LargeStringUTF8Encoding()
        {
            return "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras fermentum aliquam orci nec varius. In hac habitasse platea dictumst. Quisque congue rutrum nisi vitae malesuada. Fusce lacus odio, tempus ut lacus vel, congue venenatis urna. Vivamus nec ipsum lorem. Maecenas nisi ex, condimentum vel mauris sit amet, aliquet lacinia leo. Donec aliquet malesuada leo, eget porttitor mi gravida non. Praesent consectetur arcu sed orci luctus, ut sagittis nisl vulputate. Donec quis lectus non quam rutrum rhoncus ut non ipsum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eget massa id ex scelerisque finibus eget vel metus. Donec a semper mauris. Pellentesque eu enim sit amet tortor scelerisque bibendum."
            .ToUTF8EncodedByteArray();
        }

        [Benchmark]
        public byte[] SmallStringUTF8Encoding()
        {
            return "hello"
            .ToUTF8EncodedByteArray();
        }

        [Benchmark]
        public void GintPipeWrite()
        {
            using var pipe = new GintPipe();
            pipe.Write(buffer);
        }

        [Benchmark]
        public void GintPipeWriteUnsafe()
        {
            using var pipe = new GintPipe();
            pipe.WriteUnsafe(buffer);
        }

        [Benchmark]
        public PipeReadResult GintPipeRead()
        {
            using var pipe = new GintPipe();
            pipe.Write(buffer);
            return pipe.Read();
        }

        [Benchmark]
        public PipeReadResult GintPipeUnsafeRead()
        {
            using var pipe = new GintPipe();
            pipe.WriteUnsafe(buffer);
            return pipe.ReadUnsafe();
        }

        [Benchmark]
        public void MemoryStreamWrite()
        {
            using var stream = new MemoryStream();
            stream.Write(buffer);
        }

        [Benchmark]
        public int MemoryStreamRead()
        {
            using var stream = new MemoryStream();
            stream.Write(buffer);
            var byteArray = new byte[stream.Length];
            return stream.Read(byteArray, 0, (int)stream.Length - 1);
        }

    }
}
