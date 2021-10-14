namespace Gint
{
    internal sealed class CommandExpressionTree
    {
        public CommandExpressionTree(DiagnosticCollection diagnostics, ExpressionSyntax root, CommandSyntaxToken endToken, string command)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndToken = endToken;
            Command = command;
        }

        public DiagnosticCollection Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public CommandSyntaxToken EndToken { get; }
        public string Command { get; }

        public static CommandExpressionTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }
}
