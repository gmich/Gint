namespace Gint.SyntaxHighlighting
{
    internal sealed class ExpressionRenderItem : RenderItem
    {
        public ExpressionRenderItem(TextSpan location, CommandTokenKind kind, ExpressionSyntax node) : base(location)
        {
            Node = node;
            Kind = kind;
        }

        public ExpressionSyntax Node { get; }
        public CommandTokenKind Kind { get; }
        public override RenderItemType RenderItemType => RenderItemType.ExpressionSyntax;
    }


}
