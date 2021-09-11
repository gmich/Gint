namespace Gint
{
    internal sealed class BoundCommand : BoundNode
    {
        public BoundCommand(Command cmd, CommandSyntaxToken token, BoundOptionExpression[] boundOptions)
        {
            Command = cmd;
            TextSpan = token.Span;
            BoundOptions = boundOptions;
        }

        public Command Command { get; }
        public TextSpan TextSpan { get; }
        public BoundOptionExpression[] BoundOptions { get; }
        public override BoundNodeKind Kind { get; } = BoundNodeKind.Command;
    }

}
