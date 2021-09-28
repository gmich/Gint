namespace Gint.Markup
{
    public static class MarkupDocumentExtensions
    {
        public static MarkupDocument NewLine(this MarkupDocument document)
        {
            document.AddFormatToken("br");
            return document;
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
    }
}
