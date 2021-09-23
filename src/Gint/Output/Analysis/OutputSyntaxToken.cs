namespace Gint
{
    public sealed class OutputSyntaxToken 
    {
        public OutputSyntaxToken(OutputTokenKind kind, int position, string text, string value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public OutputTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public string Value { get; }
        public TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);
    }
}
