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
                        ReadText();
                        break;
                }
            }

            var length = position - start;
            var text = this.text.Substring(start, length);

            return new MarkupSyntaxToken(kind, start, text, value);
        }

        private void ReadFormat()
        {
            TextSpan GetSpan()
            {
                var length = position - start;
                return new TextSpan(start, length);
            }

            if (!Current.HasValue)
            {
                Diagnostics.ReportNullAtStartOfFormat(GetSpan());
                return;
            }

            if (Current.Value == MarkupFormatConsts.FormatEnd)
            {
                position++;
                kind = MarkupTokenKind.FormatEnd;
            }
            else
                kind = MarkupTokenKind.FormatStart;

            while (Current.HasValue)
            {
                if (Current.Value == MarkupFormatConsts.FormatSeparator)
                {
                    readingFormat = true;
                    position++;
                    return;
                }
                else if (Current.Value == MarkupFormatConsts.FormatTagClose)
                {
                    readingFormat = false;
                    position++;
                    return;
                }
                else
                {
                    value += Current;
                    position++;
                }
            }
            readingFormat = false;
            Diagnostics.ReportUnterminatedFormat(GetSpan());
        }

        private void ReadText()
        {
            while (
                Current != MarkupFormatConsts.FormatTagOpen
                && Current != '\r'
                && Current != '\n'
                && Current.HasValue)
            {
                value += Current;
                position++;
            }
            kind = MarkupTokenKind.Text;
        }

    }
}
