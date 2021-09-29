using System.Collections.Generic;

namespace Gint.Markup
{
    public class FluentDocument
    {
        private readonly MarkupDocument document;
        private readonly Stack<string> tokens;
        private readonly Type formatType;

        internal enum Type
        {
            Foreground,
            Background
        }

        internal FluentDocument(Stack<string> tokens, Type type, MarkupDocument document)
        {
            formatType = type;
            this.document = document;
            this.tokens = tokens;
        }

        private string FormatPrefix => formatType == Type.Foreground ? "fg" : "bg";

        private FluentDocument Color(string color)
        {
            var token = $"{FormatPrefix}.{color}";
            tokens.Push(token);
            document.AddFormat(token);
            return this;
        }
        public FluentDocument Red()
        {
            return Color("red");
        }

        public FluentDocument Green()
        {
            return Color("green");
        }

        public FluentDocument DarkGray()
        {
            return Color("darkgray");
        }

        public FluentDocument Magenta()
        {
            return Color("magenta");
        }

        public FluentDocument Yellow()
        {
            return Color("yellow");
        }

        public FluentDocument Cyan()
        {
            return Color("cyan");
        }

        public FluentDocument DarkYellow()
        {
            return Color("darkyellow");
        }

        public FluentDocument AndForeground()
        {
            return new FluentDocument(tokens, Type.Foreground, document);
        }

        public FluentDocument AndBackground()
        {
            return new FluentDocument(tokens, Type.Background, document);
        }

        public MarkupDocument Write(string text)
        {
            document.Write(text);
            return End();
        }

        public MarkupDocument End()
        {
            foreach (string token in tokens)
            {
                document.CloseFormat(token);
            }
            return document;
        }

        public MarkupDocument WriteLine(string text)
        {
            Write(text);
            document.WriteLine();
            return document;
        }

    }
}
