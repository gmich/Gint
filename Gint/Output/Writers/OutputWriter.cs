using System;
using System.Collections.Generic;

namespace Gint
{
    public abstract class OutputWriter
    {
        internal static IEnumerable<OutputSyntaxToken> ParseTokens(string text)
        {
            var lexer = new OutputLexer(text);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == OutputTokenKind.EndOfStream)
                    break;

                yield return token;
            }
        }

        public void Print(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            StartOfStream();
            foreach(var token in ParseTokens(text))
            {
                switch (token.Kind)
                {
                    case OutputTokenKind.Text:
                        PrintText(token);
                        break;
                    case OutputTokenKind.Format:
                        Format(token);
                        break;
                    case OutputTokenKind.NewLine:
                        NewLine(token);
                        break;
                    case OutputTokenKind.EndOfStream:
                        EndOfStream(token);
                        break;

                }
            }
        }

        public abstract void Flush();
        protected abstract void StartOfStream();
        protected abstract void EndOfStream(OutputSyntaxToken token);
        protected abstract void NewLine(OutputSyntaxToken token);
        protected abstract void Format(OutputSyntaxToken token);
        protected abstract void PrintText(OutputSyntaxToken token);
    }

}
