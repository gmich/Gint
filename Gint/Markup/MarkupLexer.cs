using System.Collections.Generic;

namespace Gint.Markup
{
    internal sealed class MarkupLexer
    {
        private readonly string text;
        private int position;
        private int start;
        private string value;
        private MarkupTokenKind kind;
        private bool readingFormat;
        private bool readingVariable;

        public DiagnosticCollection Diagnostics { get; } = new DiagnosticCollection();

        public MarkupLexer(string text)
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

        public static IEnumerable<MarkupSyntaxToken> Tokenize(string text, out DiagnosticCollection diagnostics)
        {
            var lexer = new MarkupLexer(text);
            var tokens = new List<MarkupSyntaxToken>();

            while (true)
            {
                var token = lexer.Lex();
                tokens.Add(token);

                if (token.Kind == MarkupTokenKind.EndOfStream)
                    break;
            }
            diagnostics = lexer.Diagnostics;
            return tokens;
        }

        public MarkupSyntaxToken Lex()
        {
            start = position;
            value = null;
            kind = MarkupTokenKind.Text;

            if (readingFormat)
            {
                ReadFormat();
            }
            else if (readingVariable)
            {
                ReadFormatVariable();
            }
            else
            {
                switch (Current)
                {
                    case null:
                        kind = MarkupTokenKind.EndOfStream;
                        break;
                    case MarkupFormatConsts.FormatTagOpen:
                        if (Lookahead == MarkupFormatConsts.FormatTagOpen)
                        {
                            position += 2; //eat it
                            value = MarkupFormatConsts.FormatTagOpen.ToString();
                            ReadText();
                        }
                        else
                        {
                            position++;
                            ReadFormat();
                        }
                        break;
                    case '\r':
                        if (Lookahead == '\n')
                        {
                            kind = MarkupTokenKind.NewLine;
                            position += 2;
                        }
                        else
                        {
                            kind = MarkupTokenKind.NewLine;
                            position++;
                        }
                        break;
                    case '\n':
                        kind = MarkupTokenKind.NewLine;
                        position++;
                        break;
                    default:
                        if (char.IsWhiteSpace(Current.Value))
                            ReadWhiteSpace();
                        else
                            ReadText();
                        break;
                }
            }

            var text = GetText();

            return new MarkupSyntaxToken(kind, start, text, value);
        }

        private void ReadFormatVariable()
        {
            kind = MarkupTokenKind.FormatVariable;

            if (!Current.HasValue)
            {
                Diagnostics.ReportNullAtStartOfFormat(GetSpan());
                readingVariable = false;
                return;
            }

            while (Current.HasValue)
            {
                switch (Current.Value)
                {
                    case MarkupFormatConsts.FormatSeparator:
                        if (Lookahead == MarkupFormatConsts.FormatSeparator)
                        {
                            value += Current;
                            position += 2; //eat one
                            break;
                        }
                        else
                        {
                            readingFormat = true;
                            readingVariable = false;
                            position++;
                            return;
                        }
                    case MarkupFormatConsts.FormatTagClose:
                        if (Lookahead == MarkupFormatConsts.FormatTagClose)
                        {
                            value += Current;
                            position += 2; //eat one
                            break;
                        }
                        else
                        {
                            readingVariable = false;
                            position++;
                            return;
                        }
                    default:
                        value += Current;
                        position++;
                        break;
                }
            }

            readingVariable = false;
            Diagnostics.ReportUnterminatedFormat(GetSpan(), GetText());
        }

        private TextSpan GetSpan()
        {
            var length = position - start;
            return new TextSpan(start, length);
        }

        private string GetText()
        {
            var length = position - start;
            var text = this.text.Substring(start, length);
            return text;
        }

        private void ReadFormat()
        {
            if (!Current.HasValue)
            {
                Diagnostics.ReportNullAtStartOfFormat(GetSpan());
                return;
            }

            switch (Current.Value)
            {
                case MarkupFormatConsts.FormatEnd:
                    position++;
                    kind = MarkupTokenKind.FormatEnd;
                    break;
                case MarkupFormatConsts.FormatToken:
                    position++;
                    kind = MarkupTokenKind.FormatToken;
                    break;
                default:
                    kind = MarkupTokenKind.FormatStart;
                    break;
            }

            while (Current.HasValue)
            {
                switch (Current.Value)
                {
                    case MarkupFormatConsts.FormatSeparator:
                        readingFormat = true;
                        position++;
                        return;
                    case MarkupFormatConsts.FormatTagClose:
                        readingFormat = false;
                        position++;
                        return;
                    case MarkupFormatConsts.FormatVariableStart:
                        readingFormat = false;
                        readingVariable = true;
                        position++;
                        return;
                    default:
                        value += Current;
                        position++;
                        break;
                }
            }

            readingFormat = false;
            Diagnostics.ReportUnterminatedFormat(GetSpan(), GetText());
        }

        private void ReadText()
        {
            while (
                Current != MarkupFormatConsts.FormatTagOpen
                && Current != '\r'
                && Current != '\n'
                && Current.HasValue
                && !char.IsWhiteSpace(Current.Value))
            {
                value += Current;
                position++;
            }
            kind = MarkupTokenKind.Text;
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
                        {
                            value += Current;
                            position++;
                        }
                        break;
                }
            }

            kind = MarkupTokenKind.WhiteSpace;
        }

    }
}
