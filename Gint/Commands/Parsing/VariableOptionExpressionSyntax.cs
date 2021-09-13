using System.Collections.Generic;

namespace Gint
{
    internal sealed class VariableOptionExpressionSyntax : ExpressionSyntax
    {
        public VariableOptionExpressionSyntax(CommandSyntaxToken optionToken, CommandSyntaxToken variableToken)
        {
            OptionToken = optionToken;
            VariableToken = variableToken;
        }

        public override CommandTokenKind Kind => CommandTokenKind.VariableOptionExpression;
        public CommandSyntaxToken OptionToken { get; }
        public CommandSyntaxToken VariableToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OptionToken;
            yield return VariableToken;
        }
    }
}
