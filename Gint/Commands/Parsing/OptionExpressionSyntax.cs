using System.Collections.Generic;

namespace Gint
{
    internal sealed class OptionExpressionSyntax : ExpressionSyntax
    {
        public OptionExpressionSyntax(CommandSyntaxToken optionToken)
        {
            OptionToken = optionToken;
        }

        public override CommandTokenKind Kind => CommandTokenKind.OptionExpression;
        public CommandSyntaxToken OptionToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OptionToken;
        }
    }
}
