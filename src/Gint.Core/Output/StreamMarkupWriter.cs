using Gint.Markup;
using System;
using System.IO;
using System.Text;
using Gint.Pipes;

namespace Gint
{
    public class StreamMarkupWriter : MarkupWriter
    {
        private readonly Func<Stream> streamGetter;
        private Stream stream;

        public StreamMarkupWriter(Func<Stream> streamGetter)
        {
            this.streamGetter = streamGetter;
        }


        public override void Flush()
        {
            stream.Close();
            stream.Dispose();
        }

        protected override void Bind(MarkupSyntaxToken[] tokens, string text, Markup.DiagnosticCollection diagnostics)
        {
        }

        protected override void EndOfStream()
        {
            stream.Close();
            stream.Dispose();
        }

        protected override void FormatEnd(string tag)
        {
           
        }

        protected override void FormatStart(string tag, string variable)
        {
            
        }

        protected override void FormatToken(string tag, string variable)
        {
            
        }

        protected override void NewLine()
        {
            stream.Write(Environment.NewLine.ToUTF8EncodedByteArray());
        }

        protected override bool OnLintingError(Markup.DiagnosticCollection diagnostics, string text)
        {
            return true;
        }

        protected override void PrintText(string text)
        {
            stream.Write(text.ToUTF8EncodedByteArray());
        }

        protected override void PrintWhitespace(string whitespace)
        {
            stream.Write(whitespace.ToUTF8EncodedByteArray());
        }

        protected override void StartOfStream()
        {
            stream = streamGetter();
        }
    }

}
