namespace Gint
{
    public enum CommandTokenKind
    {
        Option,
        Keyword,
        Pipe,
        WhiteSpace,
        End,

        CommandExpression,
        CommandWithVariableExpression,
        OptionExpression,
        VariableOptionExpression,
        PipeExpression,
        PipedCommandExpression

    }

}
