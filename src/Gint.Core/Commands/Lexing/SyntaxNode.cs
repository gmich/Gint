using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Gint.Markup;

namespace Gint
{
    internal abstract class SyntaxNode
    {
        public abstract CommandTokenKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public virtual IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    foreach (var child in children)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        public CommandSyntaxToken GetLastToken()
        {
            if (this is CommandSyntaxToken token)
                return token;

            // A syntax node should always contain at least 1 token.
            return GetChildren().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            Out @out = null;
            FluentDocument doc = null;
            if (writer is OutTextWriterAdapter o)
                @out = o.Out;

            var marker = isLast ? "└──" : "├──";

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            doc = @out?.WithForegroundColor().DarkGray();

            writer.Write(indent);
            writer.Write(marker);

            doc?.End();
            if (isToConsole)
                Console.ForegroundColor = node is CommandSyntaxToken ? ConsoleColor.Yellow : ConsoleColor.Cyan;

            if (node is CommandSyntaxToken)
                doc = @out?.WithForegroundColor().Yellow();
            else
                doc = @out?.WithForegroundColor().Cyan();


            writer.Write(node.Kind);

            if (node is CommandSyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            if (isToConsole)
                Console.ResetColor();
            doc?.End();

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);

            @out?.Flush();
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }

}
