using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gint
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (var child in children)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        private IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.Name == nameof(Kind))
                    continue;
                if (property.PropertyType == typeof(TextSpan))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
                    typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var value = property.GetValue(this);
                if (value != null)
                    yield return (property.Name, value);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            Out @out = null;
            if (writer is OutTextWriterAdapter o)
                @out = o.Out;

            var marker = isLast ? "└──" : "├──";

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            @out?.Format(FormatType.ForegroundDarkGray);

            writer.Write(indent);
            writer.Write(marker);

            if (isToConsole)
                Console.ForegroundColor = (node is BoundNode) ? ConsoleColor.Cyan : ConsoleColor.Yellow;
            @out?.Format((node is BoundNode) ? FormatType.ForegroundCyan : FormatType.ForegroundYellow);

            var text = GetText(node);
            writer.Write(text);

            var isFirstProperty = true;

            foreach (var p in node.GetProperties())
            {
                if (isFirstProperty)
                    isFirstProperty = false;
                else
                {
                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    @out?.Format(FormatType.ForegroundDarkGray);

                    writer.Write(",");
                }

                writer.Write(" ");

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                @out?.Format(FormatType.ForegroundYellow);

                writer.Write(p.Name);

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                @out?.Format(FormatType.ForegroundDarkGray);

                writer.Write(" = ");

                if (isToConsole)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                @out?.Format(FormatType.ForegroundDarkYellow);

                writer.Write(p.Value);
            }

            if (isToConsole)
                Console.ResetColor();
            @out?.ClearFormat();

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);

            @out?.Flush();
        }

        private static string GetText(BoundNode node)
        {
            return node.Kind.ToString();
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
