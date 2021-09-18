using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Markup
{
    class MarkupLinter
    {
        private readonly MarkupSyntaxToken[] tokens;
        private readonly DiagnosticCollection diagnostics = new();

        private MarkupLinter(string text)
        {
            tokens = MarkupLexer.Tokenize(text, out var lexerDiagnostics).ToArray();
            diagnostics.AddRange(lexerDiagnostics);
        }

        public static MarkupSyntaxToken[] Lint(string text, out DiagnosticCollection diagnostics)
        {
            var linter = new MarkupLinter(text);
            var formatTokens = new List<MarkupSyntaxToken>();

            foreach (var token in linter.tokens)
            {
                switch (token.Kind)
                {
                    case MarkupTokenKind.Text:
                        break;
                    case MarkupTokenKind.FormatStart:
                        formatTokens.Add(token);
                        break;
                    case MarkupTokenKind.FormatEnd:
                        var tokenToRemove = formatTokens.Where(c => c.Value == token.Value).FirstOrDefault();
                        if (tokenToRemove == null)
                            linter.diagnostics.ReportMissingStartTag(token.Span,token.Value);
                        else
                            formatTokens.Remove(tokenToRemove);
                        break;
                    case MarkupTokenKind.NewLine:
                        break;
                    case MarkupTokenKind.EndOfStream:
                        break;
                }
            }

            foreach (var token in formatTokens)
            {
                linter.diagnostics.ReportMissingEndTag(token.Span, token.Value);
            }

            diagnostics = linter.diagnostics;
            return linter.tokens;
        }
    }
}