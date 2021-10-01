using System.Collections.Generic;

namespace Gint
{
    using Markup;

    public static class MarkupDocumentExtensions
    {
        public static MarkupDocument WriteLine(this MarkupDocument document)
        {
            document.AddFormatToken("br");
            return document;
        }

        public static MarkupDocument WriteLine(this MarkupDocument document, string text)
        {
            document.Write(text);
            return document.WriteLine();
        }

        public static MarkupDocument Timestamp(this MarkupDocument document)
        {
            document.AddFormatTokenWithVariable("date", "HH:mm:ss");
            return document;
        }

        public static MarkupDocument LongTimestamp(this MarkupDocument document)
        {
            document.AddFormatTokenWithVariable("date", "yyyy-MM-dd-THH:mm:ss");
            return document;
        }

        public static MarkupDocument WriteWhitespace(this MarkupDocument document)
        {
            document.Write(" ");
            return document;
        }

        public static MarkupDocument Intent(this MarkupDocument document)
        {
            document.Write("    ");
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
            return new FluentDocument(new Stack<CloseFormat>(), FluentDocument.Type.Foreground, document);
        }

        public static FluentDocument WithBackgroundColor(this MarkupDocument document)
        {
            return new FluentDocument(new Stack<CloseFormat>(), FluentDocument.Type.Background, document);
        }
    }
}
