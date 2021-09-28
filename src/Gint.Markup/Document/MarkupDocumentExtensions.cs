using System.Collections.Generic;

namespace Gint.Markup
{
    public static class MarkupDocumentExtensions
    {
        public static MarkupDocument WriteLine(this MarkupDocument document)
        {
            document.AddFormatToken("br");
            return document;
        }

        public static MarkupDocument WriteLine(this MarkupDocument document,string text)
        {
            document.Write(text);
            return document.WriteLine();
        }

        public static MarkupDocument Timestamp(this MarkupDocument document)
        {
            document.AddFormatTokenWithVariable("date", "HH:mm:ss");
            return document;
        }

        public static MarkupDocument Whitespace(this MarkupDocument document)
        {
            document.Write(" ");
            return document;
        }

        public static MarkupDocument WriteWithinFormat(this MarkupDocument document, string format, string text)
        {
            var close = document.AddFormat(format);
            document.Write(text);
            close.Close();
            return document;
        }

        public static FluentDocument WithForegroundColor(this MarkupDocument document)
        {
            return new FluentDocument(new Stack<string>(), FluentDocument.Type.Foreground,document);
        }
    }

    public class FluentDocument
    {
        private readonly MarkupDocument document;
        private readonly Stack<string> tokens;
        internal Type FormatType { get; }
        internal enum Type
        {
            Foreground,
            Background
        }

        internal FluentDocument(Stack<string> tokens, Type type, MarkupDocument document)
        {
            FormatType = type;
            this.document = document;
            this.tokens = tokens;
        }

        private string FormatPrefix => FormatType == Type.Foreground ? "fg" : "bg";

        public FluentDocument Red()
        {
            var token = $"{FormatPrefix}.red";
            tokens.Push(token);
            document.AddFormat(token);
            return this;
        }

        public FluentDocument Green()
        {
            var token = $"{FormatPrefix}.green";
            tokens.Push(token);
            document.AddFormat(token);
            return this;
        }

        public FluentDocument Magenta()
        {
            var token = $"{FormatPrefix}.magenta";
            tokens.Push(token);
            document.AddFormat(token);
            return this;
        }

        public FluentDocument AndForeground()
        {
            return new FluentDocument(tokens, Type.Foreground, document);
        }

        public FluentDocument AndBackground()
        {
            return new FluentDocument(tokens, Type.Background, document);
        }

        public MarkupDocument Write(string text)
        {
            document.Write(text);
            CloseFormat();
            return document;
        }

        private void CloseFormat()
        {
            foreach (string token in tokens)
            {
                document.CloseFormat(token);
            }
        }

        public MarkupDocument WriteLine(string text)
        {
            Write(text);
            document.WriteLine();
            return document;
        }
    }

    public partial class FormatType
    {
        public partial class Foreground
        {

        }
    }
}
