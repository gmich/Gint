using System;
using System.Text;

namespace Gint
{
    internal class ExecutionBuffer
    {
        public StringBuilder Builder { get; } = new StringBuilder();

        public override string ToString()
        {
            return Builder.ToString();
        }

        public string Drain()
        {
            var text= Builder.ToString();
            Builder.Clear();
            return text;
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
        private readonly StringBuilder builder = new StringBuilder();

        public BufferOutputWriter(ExecutionBuffer buffer)
        {
            this.buffer = buffer;
        }

        public override void Flush()
        {
            buffer.Builder.Append(builder.ToString());
            builder.Clear();
        }

        protected override void Format(OutputSyntaxToken token)
        {
        }

        protected override void NewLine(OutputSyntaxToken token)
        {
            builder.AppendLine();
        }

        protected override void PrintText(OutputSyntaxToken token)
        {
            builder.Append(token.Value);
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
