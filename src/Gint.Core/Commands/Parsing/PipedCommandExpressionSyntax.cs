using System.Collections.Generic;

namespace Gint
{
    internal sealed class PipedCommandExpressionSyntax : ExpressionSyntax
    {
        public PipedCommandExpressionSyntax(ExpressionSyntax firstCommand, PipeExpressionSyntax pipeToken, ExpressionSyntax secondCommand)
        {
            FirstCommand = firstCommand;
            PipeToken = pipeToken;
            SecondCommand = secondCommand;
        }

        public ExpressionSyntax FirstCommand { get; }
        public PipeExpressionSyntax PipeToken { get; }
        public ExpressionSyntax SecondCommand { get; }

        public override CommandTokenKind Kind => CommandTokenKind.PipedCommandExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return FirstCommand;
            yield return PipeToken;
            yield return SecondCommand;
        }
    }
}
