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
                var shouldContinue = OnLintingError(diagnostics);
                if (!shouldContinue)
                    return;
            }

            StartOfStream();
            foreach(var token in tokens)
            {
                switch (token.Kind)
                {
                    case MarkupTokenKind.Text:
                        PrintText(token);
                        break;
                    case MarkupTokenKind.FormatStart:
                        FormatStart(token);
                        break;
                    case MarkupTokenKind.FormatEnd:
                        FormatEnd(token);
                        break;
                    case MarkupTokenKind.NewLine:
                        NewLine(token);
                        break;

                }
            }
            EndOfStream();
        }

        public abstract bool OnLintingError(DiagnosticCollection diagnostics);
        public abstract void Flush();
        protected abstract void StartOfStream();
        protected abstract void EndOfStream();
        protected abstract void NewLine(MarkupSyntaxToken token);
        protected abstract void FormatStart(MarkupSyntaxToken token);
        protected abstract void FormatEnd(MarkupSyntaxToken token);
        protected abstract void PrintText(MarkupSyntaxToken token);
    }

}
