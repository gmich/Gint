namespace Gint
{

    internal sealed class BoundPipe : BoundNode
    {
        public BoundPipe(PipeExpressionSyntax syntax)
        {
            TextSpan = syntax.Span;
        }
        public override BoundNodeKind Kind { get; } = BoundNodeKind.Pipe;
        public TextSpan TextSpan { get; }
    }

}
