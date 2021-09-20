using System.Collections.Generic;
using System.Linq;

namespace Gint.Markup
{
    public abstract class MarkupWriter
    {
        public void Print(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var tokens = MarkupLinter.Lint(text, out var diagnostics);
            Bind(tokens, text, diagnostics);
            if (diagnostics.Any())
            {
                var shouldContinue = OnLintingError(diagnostics, text);
                if (!shouldContinue)
                    return;
            }

            StartOfStream();
            var formatTokens = new Stack<(string Token, string Variable)>();


            string GetVariableIfExists(int current)
            {
                var next = current + 1;
                if (next < tokens.Length
                    && tokens[next].Kind == MarkupTokenKind.FormatVariable)
                    return tokens[next].Value;
                else
                    return string.Empty;
            }

            for (int i = 0; i < tokens.Length; i++)
            {
                MarkupSyntaxToken token = tokens[i];
                switch (token.Kind)
                {
                    case MarkupTokenKind.Text:
                        PrintText(token.Value);
                        break;
                    case MarkupTokenKind.FormatStart:
                        var var = GetVariableIfExists(i);
                        formatTokens.Push((token.Value, var));
                        FormatStart(token.Value, var);
                        break;
                    case MarkupTokenKind.FormatEnd:
                        if (string.IsNullOrEmpty(token.Value))
                        {
                            var formatStart = formatTokens.Pop();
                            FormatEnd(formatStart.Token, string.IsNullOrEmpty(formatStart.Variable) ? GetVariableIfExists(i) : formatStart.Variable);
                        }
                        else
                            FormatEnd(token.Value, GetVariableIfExists(i));
                        break;
                    case MarkupTokenKind.FormatToken:
                        FormatToken(token.Value, GetVariableIfExists(i));
                        break;
                    case MarkupTokenKind.WhiteSpace:
                        PrintWhitespace(token.Value);
                        break;
                    case MarkupTokenKind.NewLine:
                        NewLine();
                        break;

                }
            }
            EndOfStream();
        }

        protected abstract bool OnLintingError(DiagnosticCollection diagnostics, string text);
        protected abstract void Bind(MarkupSyntaxToken[] tokens, string text, DiagnosticCollection diagnostics);

        protected abstract void StartOfStream();
        protected abstract void EndOfStream();

        protected abstract void FormatStart(string tag, string variable);
        protected abstract void FormatEnd(string tag, string variable);
        protected abstract void FormatToken(string tag, string variable);

        protected abstract void NewLine();
        protected abstract void PrintWhitespace(string whitespace);
        protected abstract void PrintText(string text);
    }

}
