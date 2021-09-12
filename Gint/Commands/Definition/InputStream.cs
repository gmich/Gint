namespace Gint
{
    public class InputStream
    {
        public InputStream(string raw, string unformatted)
        {
            Raw = raw;
            Unformatted = unformatted;
        }

        public string Raw { get; }
        public string Unformatted { get; }
    }
}
