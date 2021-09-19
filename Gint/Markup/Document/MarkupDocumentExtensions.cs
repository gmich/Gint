namespace Gint.Markup
{
    public static class MarkupDocumentExtensions
    {
        public static MarkupDocument NewLine(this MarkupDocument document)
        {
            document.FormatToken("br");
            return document;
        }

        public static MarkupDocument WriteWithinRedFontWithWhiteBackground(this MarkupDocument document, string text)
        {
            var format = document.StartFormat(new[] { "fg.red", "bg.white" });
            document.Write(text);
            format.Close();
            return document;
        }

        public static MarkupDocument WriteDate(this MarkupDocument document)
        {
            document.FormatTokenWithVariable("date", "HH:mm tt");
            return document;
        }

        public static MarkupDocument CloseTag(this MarkupDocument document)
        {
            document.CloseFormat(string.Empty);
            return document;
        }
    }
}
