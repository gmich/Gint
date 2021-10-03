using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.SyntaxHighlighting.Analysis
{

    internal sealed class SyntaxHighlighterLexer
    {
        private readonly string text;
        private int position;
        private int start;
        private HighlightTokenKind kind;

        private SyntaxHighlighterLexer(string text)
        {
            this.text = text;
        }

        private char Current => Peek(0);

        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = position + offset;

            if (index >= text.Length)
                return '\0';

            return text[index];
        }

        private bool EndOfText => position >= text.Length;

        public static HighlightToken[] Tokenize(string text)
        {
            var lexer = new SyntaxHighlighterLexer(text);
            var tokens = new List<HighlightToken>();

            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == HighlightTokenKind.EOF)
                    break;

                tokens.Add(token);

            }
            return tokens.ToArray();
        }

        public HighlightToken Lex()
        {
            start = position;
            kind = HighlightTokenKind.Unknown;

            if (Current == '\0')
            {
                kind = HighlightTokenKind.EOF;
                return new HighlightToken(kind, start, string.Empty);
            }
            else if (Current == '>')
            {
                kind = HighlightTokenKind.Pipe;
                position++;
            }
            else if (Current == '"')
            {
                kind = HighlightTokenKind.DoubleQuotes;
                position++;
            }
            else if (Current == '\'')
            {
                kind = HighlightTokenKind.SingleQuote;
                position++;
            }
            else if (char.IsWhiteSpace(Current))
            {
                ReadWhiteSpace();
            }
            else
            {
                ReadIdentifierOrKeyword();
            }

            var length = position - start;
            var span = text.Substring(start, length);

            return new HighlightToken(kind, start, span);
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current) && !EndOfText)
                position++;

            kind = HighlightTokenKind.Whitespace;
        }

        private void ReadIdentifierOrKeyword()
        {
            if (Current == '-')
            {
                kind = HighlightTokenKind.Option;
            }
            else
            {
                kind = HighlightTokenKind.Keyword;
            }


            while (!char.IsWhiteSpace(Current)
                && !EndOfText
                && Current != '"'
                && Current != '\''
                && Current != ')'
                && Current != '(')
                position++;

            //var length = position - start;
            //var parsedText = text.Substring(start, length);
        }
    }
}
