namespace Gint
{
    internal class BoundOption : BoundOptionExpression
    {
        public BoundOption(int priority, Option option, OptionExpressionSyntax optionExpression)
        {
            Priority = priority;
            Option = option;
            TextSpan = optionExpression.Span;
        }
        public override int Priority { get; }
        public override BoundNodeKind Kind { get; } = BoundNodeKind.Option;
        public Option Option { get; }
        public TextSpan TextSpan { get; }
    }
}
