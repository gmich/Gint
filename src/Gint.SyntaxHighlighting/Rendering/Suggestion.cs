namespace Gint.SyntaxHighlighting
{
    internal class Suggestion
    {
        public string Suggested { get; set; }
        public string Actual { get; set; }
        public string Printed { get; set; }

        public int TotalSize => Printed.Length;
    }


}
