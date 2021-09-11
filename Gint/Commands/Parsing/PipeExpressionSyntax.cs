using System.Collections.Generic;

namespace Gint
{
    internal sealed class PipeExpressionSyntax : ExpressionSyntax
    {
        public PipeExpressionSyntax(CommandSyntaxToken pipeToken)
        {
            PipeToken = pipeToken;
        }

        public override CommandTokenKind Kind => CommandTokenKind.PipeExpression;
        public CommandSyntaxToken PipeToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return PipeToken;
        }
    }
}
