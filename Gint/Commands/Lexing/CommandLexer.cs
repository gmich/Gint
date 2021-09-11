﻿using System.Text;

namespace Gint
{
    internal sealed class CommandLexer
    {
        private readonly string text;
        private int position;
        private int start;
        private string value;
        private CommandTokenKind kind;
        public DiagnosticCollection Diagnostics { get; } = new DiagnosticCollection();

        public CommandLexer(string text)
        {
            this.text = text;
        }

        private char? Current => Peek(0);

        private char? Lookahead => Peek(1);

        private char? Peek(int offset)
        {
            var index = position + offset;

            if (index >= text.Length)
                return null;

            return text[index];
        }

        private void ReadArgumentWithinQuotes()
        {
            // Skip the first quote
            position++;

            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(start, 1);
                        Diagnostics.ReportUnterminatedString(span);
                        done = true;
                        break;
                    case '"':
                        if (Lookahead == '"')
                        {
                            sb.Append(Current);
                            position += 2;
                        }
                        else
                        {
                            position++;
                            done = true;
                        }
                        break;
                    default:
                        sb.Append(Current);
                        position++;
                        break;
                }
            }

            kind = CommandTokenKind.Keyword;
            value = sb.ToString();
        }


        public CommandSyntaxToken Lex()
        {
            start = position;
            value = null;
            kind = CommandTokenKind.Keyword;

            switch (Current)
            {
                case null:
                    kind = CommandTokenKind.End;
                    break;
                case '"':
                    ReadArgumentWithinQuotes();
                    break;
                case '>':
                    position ++;
                    value = ">";
                    kind = CommandTokenKind.Pipe;
                    break;
                case '-':
                    if (Lookahead == '-')
                    {
                        position += 2;
                        ReadKeyword();
                        kind = CommandTokenKind.Option;
                    }
                    else
                    {
                        position++;
                        ReadKeyword();
                        kind = CommandTokenKind.Option;
                    }
                    break;
                default:
                    if (char.IsWhiteSpace(Current.Value))
                    {
                        ReadWhiteSpace();
                    }
                    else
                    {
                        ReadKeyword();
                        kind = CommandTokenKind.Keyword;
                    }

                    break;
            }

            var length = position - start;
            var text = this.text.Substring(start, length);

            return new CommandSyntaxToken(kind, start, text, value);
        }

        private void ReadWhiteSpace()
        {
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case null:
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(Current.Value))
                            done = true;
                        else
                            position++;
                        break;
                }
            }
            var length = position - start;
            value = text.Substring(start, length);

            kind = CommandTokenKind.WhiteSpace;
        }

        private void ReadKeyword()
        {
            while (Current.HasValue && (char.IsLetterOrDigit(Current.Value) || Current == '-' || Current == '>'))
                position++;

            var length = position - start;
            value = text.Substring(start, length);
        }


    }
}
