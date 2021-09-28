using Gint.Markup;
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

    internal class BufferOutputWriter : MarkupWriter
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

        protected override void StartOfStream()
        {
        }

        protected override bool OnLintingError(Markup.DiagnosticCollection diagnostics, string text)
        {
            return true;
        }

        protected override void Bind(MarkupSyntaxToken[] tokens, string text, Markup.DiagnosticCollection diagnostics)
        {
            foreach (var token in tokens)
            {
                if (token.Kind == MarkupTokenKind.Text
                || token.Kind == MarkupTokenKind.WhiteSpace
                || token.Kind == MarkupTokenKind.NewLine)
                {
                    builderWithoutFormat.Append(token.Text);
                    rawBuilder.Append(token.Text);
                }
                else
                {
                    rawBuilder.Append(token.Text);
                }
            }
        }

        protected override void EndOfStream()
        {
        }

        protected override void FormatStart(string tag, string variable)
        {
        }

        protected override void FormatEnd(string tag)
        {
        }

        protected override void FormatToken(string tag, string variable)
        {
        }

        protected override void NewLine()
        {
        }

        protected override void PrintWhitespace(string whitespace)
        {
        }

        protected override void PrintText(string text)
        {
        }
    }

}
