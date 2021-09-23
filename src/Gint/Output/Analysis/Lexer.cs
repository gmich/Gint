namespace Gint
{
    internal sealed class OutputLexer
    {
        private readonly string text;
        private int position;
        private int start;
        private string value;
        private OutputTokenKind kind;

        public OutputLexer(string text)
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


        public OutputSyntaxToken Lex()
        {
            start = position;
            value = null;
            kind = OutputTokenKind.Text;
 
            switch (Current)
            {
                case null:
                    kind = OutputTokenKind.EndOfStream;
                    break;
                case FormatFacts.MaskPrefix:
                    if (Lookahead == FormatFacts.MaskPrefix)
                    {
                        position += 2; //eat it
                        value = FormatFacts.MaskPrefix.ToString();
                        ReadText();
                    }
                    else
                    {
                        ReadFormat();
                    }
                    break;
                case '\r':
                    if (Lookahead == '\n')
                    {
                        kind = OutputTokenKind.NewLine;
                        position += 2;
                    }
                    else
                    {
                        kind = OutputTokenKind.NewLine;
                        position++;
                    }
                    break;
                case '\n':
                    kind = OutputTokenKind.NewLine;
                    position++;
                    break;
                default:
                    ReadText();
                    break;
            }

            var length = position - start;
            var text = this.text.Substring(start, length);
       
            return new OutputSyntaxToken(kind, start, text, value);
        }

        private void ReadFormat()
        {
            position++;
            for (int i = 0; i < FormatFacts.MaskLength; i++)
            {
                if (Current.HasValue)
                {
                    value += Current;
                    position++;
                }
                else
                {
                    value += FormatFacts.PlaceholderChar;
                }
            }

            kind = OutputTokenKind.Format;
        }

        private void ReadText()
        {
            while (Current != FormatFacts.MaskPrefix
                && Current != '\r'
                && Current != '\n'
                && Current.HasValue)
            {
                value += Current;
                position++;
            }
            kind = OutputTokenKind.Text;
        }

    }
}
