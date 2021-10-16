using System;
using System.Collections.Generic;

namespace Gint
{
    public class FluentDocument
    {
        private readonly MarkupDocument document;
        private readonly Stack<CloseFormat> tokens;
        private readonly Type formatType;

        internal enum Type
        {
            Foreground,
            Background
        }

        internal FluentDocument(Stack<CloseFormat> tokens, Type type, MarkupDocument document)
        {
            formatType = type;
            this.document = document;
            this.tokens = tokens;
        }

        private string FormatPrefix => formatType == Type.Foreground ? "fg" : "bg";

        private FluentDocument Color(string color)
        {
            var close = document.AddFormatWithVariable(FormatPrefix, color);
            tokens.Push(close);
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

        public FluentDocument White()
        {
            return Color("white");
        }

        public FluentDocument Black()
        {
            return Color("black");
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
            foreach (var token in tokens)
            {
                token.Close();
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
