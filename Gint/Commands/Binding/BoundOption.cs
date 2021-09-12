namespace Gint
{
    internal class BoundOption : BoundOptionExpression
    {
        public BoundOption(string argument, int priority, bool allowMultiple, Option option, OptionExpressionSyntax optionExpression) 
            : base(argument, allowMultiple, optionExpression.Span)
        {
            Priority = priority;
            Option = option;
        }
        public override int Priority { get; }
        public override BoundNodeKind Kind { get; } = BoundNodeKind.Option;
        public Option Option { get; }
    }
}
