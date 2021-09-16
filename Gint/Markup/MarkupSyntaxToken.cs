namespace Gint.Markup
{
    public sealed class MarkupSyntaxToken 
    {
        public MarkupSyntaxToken(MarkupTokenKind kind, int position, string text, string value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public MarkupTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public string Value { get; }
        public TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);


        public override string ToString()
        {
            return $"Kind: <{Kind}> , Text: <{Text}> , Value: <{Value}>";
        }
    }
}
