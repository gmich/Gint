using System.Collections;
using System.Collections.Generic;

namespace Gint.Markup
{
    public sealed class DiagnosticCollection : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            this.diagnostics.AddRange(diagnostics);
        }

        private void ReportError(TextSpan location, string message)
        {
            var diagnostic = Diagnostic.Error(location, message);
            diagnostics.Add(diagnostic);
        }

        private void ReportWarning(TextSpan location, string message)
        {
            var diagnostic = Diagnostic.Warning(location, message);
            diagnostics.Add(diagnostic);
        }

        internal void ReportNullAtStartOfFormat(TextSpan span)
        {
            var message = $"Unterminated format. Will be treated as an empty keyword";
            ReportWarning(span, message);
        }
        internal void ReportUnterminatedFormat(TextSpan span)
        {
            var message = $"Unterminated format. Will be treated as a keyword";
            ReportWarning(span, message);
        }

        internal void ReportMissingStartTag(TextSpan span)
        {
            var message = $"Missing start tag.";
            ReportWarning(span, message);
        }

        internal void ReportMissingEndTag(TextSpan span)
        {
            var message = $"Missing end tag.";
            ReportWarning(span, message);
        }

        internal void MarkdownDoesnt(TextSpan span)
        {
            var message = $"Missing end tag.";
            ReportWarning(span, message);
        }
    }
}
