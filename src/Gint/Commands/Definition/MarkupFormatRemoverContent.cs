namespace Gint
{
    public class MarkupFormatRemoverContent
    {
        public MarkupFormatRemoverContent(string raw, string unformatted)
        {
            Raw = raw;
            Unformatted = unformatted;
        }

        public string Raw { get; }
        public string Unformatted { get; }

        public bool IsEmpty => string.IsNullOrEmpty(Raw);
    }
}
