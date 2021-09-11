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
        OptionExpression,
        VariableOptionExpression,
        PipeExpression,
        PipedCommandExpression

    }

}
