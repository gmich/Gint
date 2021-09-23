using System.Collections;
using System.Collections.Generic;

namespace Gint.Markup
{
    public sealed class DiagnosticCollection : IEnumerable<MarkdownDiagnostic>
    {
        private readonly List<MarkdownDiagnostic> diagnostics = new List<MarkdownDiagnostic>();

        public IEnumerator<MarkdownDiagnostic> GetEnumerator() => diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(IEnumerable<MarkdownDiagnostic> diagnostics)
        {
            this.diagnostics.AddRange(diagnostics);
        }

        private void Report(MarkdownDiagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }

        internal void ReportUnterminatedFormat(TextSpan span, string format)
        {
            var message = $"Unterminated format. Will be treated as a keyword";
            Report(new UnterminatedFormat(DiagnosticSeverity.Warning, span, message, format));
        }

        internal void ReportMissingStartTag(TextSpan span, string tag)
        {
            var message = $"Missing start tag.";
            Report(new MissingTag(DiagnosticSeverity.Error, span, message, tag, MissingTag.TagPosition.Start));

        }

        internal void ReportMissingEndTag(TextSpan span, string tag)
        {
            var message = $"Missing end tag.";
            Report(new MissingTag(DiagnosticSeverity.Error, span, message, tag, MissingTag.TagPosition.End));
        }

        internal void ReportEndTagsCannotHaveVariables(TextSpan span)
        {
            var message = $"End tags cannot have variables.";
            Report(new EndTagWithVariable(DiagnosticSeverity.Warning, span, message));
        }

        internal void ReportUnterminatedFormatVariable(TextSpan span, string variable)
        {
            var message = $"Unterminated format variable.";
            Report(new UnterminatedFormatVariable(DiagnosticSeverity.Error, span, message, variable));
        }
    }
}
