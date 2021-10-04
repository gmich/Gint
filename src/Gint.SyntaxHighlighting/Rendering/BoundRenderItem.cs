namespace Gint.SyntaxHighlighting
{
    internal sealed class BoundRenderItem : RenderItem
    {
        public BoundRenderItem(TextSpan location, BoundNodeKind nodeKind, BoundNode node) : base(location)
        {
            Node = node;
            BoundNodeKind = nodeKind;
        }

        public BoundNode Node { get; }
        public BoundNodeKind BoundNodeKind { get; }
        public override RenderItemType RenderItemType => RenderItemType.BoundNode;
    }


}
