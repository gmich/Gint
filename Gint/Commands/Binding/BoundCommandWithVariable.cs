namespace Gint
{
    internal sealed class BoundCommandWithVariable : BoundCommand
    {
        public BoundCommandWithVariable(Command cmd, CommandSyntaxToken token,string variable, TextSpan textSpanWithVariable, BoundOptionExpression[] boundOptions) : base(cmd,token,boundOptions)
        {
            Variable = variable;
            TextSpanWithVariable = textSpanWithVariable;
        }

        public override BoundNodeKind Kind { get; } = BoundNodeKind.CommandWithVariable;
        public string Variable { get; }
        public TextSpan TextSpanWithVariable { get; }
    }

}
