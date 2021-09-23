using System.Collections.Generic;

namespace Gint
{
    internal sealed class CommandWithVariableExpressionSyntax : CommandExpressionSyntax
    {
        public CommandWithVariableExpressionSyntax(CommandSyntaxToken commandToken,CommandSyntaxToken variableToken, ExpressionSyntax[] options) : base(commandToken,options)
        {
            VariableToken = variableToken;
        }

        public override CommandTokenKind Kind => CommandTokenKind.CommandWithVariableExpression;
        public CommandSyntaxToken VariableToken { get; }

        public override TextSpan Span
        {
            get
            {
                var first = CommandToken.Span;
                var last = VariableToken.Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;
            yield return VariableToken;

            foreach (var option in Options)
            {
                yield return option;
            }
        }
    }
}
