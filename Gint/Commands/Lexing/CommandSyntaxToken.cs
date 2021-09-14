using System.Collections.Generic;
using System.Linq;

namespace Gint
{
    internal sealed class CommandSyntaxToken : SyntaxNode
    {
        public CommandSyntaxToken(CommandTokenKind kind, int position, string text, string value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override CommandTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public string Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text?.Length ?? 0);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

}
