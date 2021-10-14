using System.Text;

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

        private bool ReachedEnd => (position >= text.Length);

        private void ReadArgumentWithinCharacter(char ch)
        {
            // Skip the first quote
            position++;

            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                if (Current == ch)
                {
                    if (Lookahead == ch)
                    {
                        sb.Append(Current);
                        position += 2;
                    }
                    else
                    {
                        position++;
                        done = true;
                    }
                    continue;
                }

                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(start, 1);
                        Diagnostics.ReportUnterminatedString(span);
                        done = true;
                        break;
                    case null:
                        break;
                    default:
                        sb.Append(Current);
                        position++;
                        break;
                }
                if (ReachedEnd && !done)
                {
                    done = true;
                    var length = position - start;
                    var span = new TextSpan(start, length);
                    Diagnostics.ReportUnterminatedString(span);
                }
            }

            kind = CommandTokenKind.Keyword;
            value = sb.ToString();
        }

        private void ReadArgumentWithinApostrophe()
        {
            ReadArgumentWithinCharacter('\'');
        }

        private void ReadArgumentWithinQuotes()
        {
            ReadArgumentWithinCharacter('"');
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
                case '\'':
                    ReadArgumentWithinApostrophe();
                    break;
                case '>':
                    if (Lookahead == ' ' || Lookahead == null)
                    {
                        position++;
                        kind = CommandTokenKind.Pipe;
                        value = ">";
                        break;
                    }
                    else
                    {
                        ReadKeyword();
                        break;
                    }
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
            while (Current.HasValue && (!char.IsWhiteSpace(Current.Value)))
                position++;

            var length = position - start;
            value = text.Substring(start, length);
        }


    }
}
