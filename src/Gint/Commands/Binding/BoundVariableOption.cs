namespace Gint
{
    internal sealed class BoundVariableOption : BoundOptionExpression
    {
        public BoundVariableOption(string argument, int priority, bool allowMultiple, VariableOption option, string variable, VariableOptionExpressionSyntax optionExpression) 
            : base(argument, allowMultiple, optionExpression.Span)
        {
            Priority = priority;
            VariableOption = option;
            TextSpanWithVariable = TextSpan.FromBounds(TextSpan.Start, optionExpression.VariableToken.Span.End);
            Variable = variable;
        }

        public override BoundNodeKind Kind { get; } = BoundNodeKind.VariableOption;
        public override int Priority { get; }
        public VariableOption VariableOption { get; }
        public string Variable { get; }
        public TextSpan TextSpanWithVariable { get; }
    }
}
