using System;
using Gint;
using Gint.SyntaxHighlighting.Analysis;

namespace Gint.SyntaxHighlighting
{

    internal class Suggestion
    {
        public string Suggested { get; set; }
        public string Actual { get; set; }
        public string Printed { get; set; }
        public string PrintedDetailed { get; set; }
    }


    internal class CommandRenderer
    {
        private void RenderInternal(string text)
        {
            var tokens = SyntaxHighlighterLexer.Tokenize(text);

            foreach (var token in tokens)
            {
                switch (token.Kind)
                {
                    case HighlightTokenKind.Unknown:
                        RenderUnknown(token);
                        break;
                    case HighlightTokenKind.Whitespace:
                        RenderWhiteSpace(token);
                        break;
                    case HighlightTokenKind.Option:
                        RenderOption(token);
                        break;
                    case HighlightTokenKind.Keyword:
                        RenderKeyword(token);
                        break;
                    case HighlightTokenKind.EOF:
                        break;
                    case HighlightTokenKind.Pipe:
                        RenderPipe(token);
                        break;
                    case HighlightTokenKind.DoubleQuotes:
                        RenderDoubleQuotes(token);
                        break;
                    case HighlightTokenKind.SingleQuote:
                        RenderSingleQuote(token);
                        break;
                }
            }

        }

        private void RenderUnknown(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.White);
        }

        private void RenderWhiteSpace(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.White);
        }

        private void RenderOption(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Yellow);
        }

        private void RenderKeyword(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Green);
        }

        private void RenderPipe(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private void RenderDoubleQuotes(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private void RenderSingleQuote(HighlightToken token)
        {
            RenderText(token.Text, ConsoleColor.Magenta);
        }

        private void RenderText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public void Render(string command)
        {
            RenderInternal(command);
        }
    }


}
