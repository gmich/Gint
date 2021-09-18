using System;
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
            if(diagnostics.Any())
            {
                var shouldContinue = OnLintingError(diagnostics, text);
                if (!shouldContinue)
                    return;
            }

            StartOfStream();
            foreach(var token in tokens)
            {
                switch (token.Kind)
                {
                    case MarkupTokenKind.Text:
                        PrintText(token.Value);
                        break;
                    case MarkupTokenKind.FormatStart:
                        FormatStart(token.Value);
                        break;
                    case MarkupTokenKind.FormatEnd:
                        FormatEnd(token.Value);
                        break;
                    case MarkupTokenKind.FormatToken:
                        FormatToken(token.Value);
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

        public abstract bool OnLintingError(DiagnosticCollection diagnostics, string text);
        protected abstract void StartOfStream();
        protected abstract void EndOfStream();

        protected abstract void FormatStart(string tag);
        protected abstract void FormatEnd(string tag);
        protected abstract void FormatToken(string tag);

        protected abstract void NewLine();
        protected abstract void PrintWhitespace(string whitespace);
        protected abstract void PrintText(string text);
    }

}
