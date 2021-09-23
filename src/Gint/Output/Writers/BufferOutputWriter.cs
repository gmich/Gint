using System;
using System.Text;

namespace Gint
{
    internal class ExecutionBuffer
    {
        public StringBuilder RawBuilder { get; } = new StringBuilder();
        public StringBuilder BuilderWithoutFormat { get; } = new StringBuilder();

        public override string ToString()
        {
            return RawBuilder.ToString();
        }

        public InputStream Drain()
        {
            var stream = new InputStream(RawBuilder.ToString(), BuilderWithoutFormat.ToString());

            RawBuilder.Clear();
            BuilderWithoutFormat.Clear();

            return stream;
        }
    }

    internal class BufferOutputEventArgs
    {
        public BufferOutputEventArgs(ExecutionBuffer buffer)
        {
            Buffer = buffer;
        }

        public ExecutionBuffer Buffer { get; }
    }

    internal class BufferOutputWriter : OutputWriter
    {
        private readonly ExecutionBuffer buffer;
        public event EventHandler<BufferOutputEventArgs> OnBufferStreamEnd;
        private readonly StringBuilder rawBuilder = new StringBuilder();
        private readonly StringBuilder builderWithoutFormat = new StringBuilder();

        public BufferOutputWriter(ExecutionBuffer buffer)
        {
            this.buffer = buffer;
        }

        public override void Flush()
        {
            buffer.BuilderWithoutFormat.Append(builderWithoutFormat.ToString());
            buffer.RawBuilder.Append(rawBuilder.ToString());
            rawBuilder.Clear();
            builderWithoutFormat.Clear();
        }

        protected override void Format(OutputSyntaxToken token)
        {
            rawBuilder.Append(token.Text);
        }

        protected override void NewLine(OutputSyntaxToken token)
        {
            builderWithoutFormat.AppendLine();
            rawBuilder.AppendLine();
        }

        protected override void PrintText(OutputSyntaxToken token)
        {
            builderWithoutFormat.AppendLine(token.Value);
            rawBuilder.Append(token.Value);
        }

        protected override void EndOfStream(OutputSyntaxToken token)
        {
            OnBufferStreamEnd?.Invoke(this, new BufferOutputEventArgs(buffer));
        }

        protected override void StartOfStream()
        {
        }
    }

}
