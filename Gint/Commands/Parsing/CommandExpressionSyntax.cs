using System.Collections.Generic;

namespace Gint
{
    internal sealed class CommandExpressionSyntax : ExpressionSyntax
    {
        public CommandExpressionSyntax(CommandSyntaxToken commandToken, ExpressionSyntax[] options)
        {
            CommandToken = commandToken;
            Options = options;
        }

        public override CommandTokenKind Kind => CommandTokenKind.CommandExpression;
        public CommandSyntaxToken CommandToken { get; }
        public ExpressionSyntax[] Options { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;
            foreach (var option in Options)
            {
                yield return option;
            }
        }
    }
}
