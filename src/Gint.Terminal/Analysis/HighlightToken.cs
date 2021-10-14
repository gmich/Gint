namespace Gint.Terminal.Analysis
{
    internal sealed class HighlightToken
    {
        public HighlightToken(HighlightTokenKind kind, int position, string text)
        {
            Kind = kind;
            Position = position;
            Text = text;
        }

        public HighlightTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);
    }
}
