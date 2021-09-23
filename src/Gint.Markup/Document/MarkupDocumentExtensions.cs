namespace Gint.Markup
{
    public static class MarkupDocumentExtensions
    {
        public static MarkupDocument NewLine(this MarkupDocument document)
        {
            document.FormatToken("br");
            return document;
        }

        public static MarkupDocument Timestamp(this MarkupDocument document)
        {
            document.FormatTokenWithVariable("date", "HH:mm:ss");
            return document;
        }

        public static MarkupDocument Whitespace(this MarkupDocument document)
        {
            document.Write(" ");
            return document;
        }

        public static MarkupDocument CloseTag(this MarkupDocument document)
        {
            document.CloseFormat(string.Empty);
            return document;
        }
    }
}
