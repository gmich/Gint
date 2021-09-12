namespace Gint
{
    internal class BoundOption : BoundOptionExpression
    {
        public BoundOption(string argument, int priority, Option option, OptionExpressionSyntax optionExpression) : base(argument)
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
