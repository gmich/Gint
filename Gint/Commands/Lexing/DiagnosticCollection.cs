using System.Collections;
using System.Collections.Generic;

namespace Gint
{
    internal sealed class DiagnosticCollection : IEnumerable<Diagnostic>
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

        internal void ReportUnterminatedString(TextSpan span)
        {
            var message = "Unterminated string literal.";
            ReportError(span, message);
        }

        internal void ReportUnexpectedToken(TextSpan span, CommandTokenKind actualKind, CommandTokenKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            ReportError(span, message);
        }

        internal void ReportCommandUnknown(TextSpan span, string cmd)
        {
            var message = $"Unknown command <{cmd}>.";
            ReportError(span, message);
        }

        internal void ReportOptionUnknown(TextSpan span, string option)
        {
            var message = $"Unknown option <{option}>.";
            ReportError(span, message);
        }

        internal void ReportUnecessaryVariable(TextSpan span)
        {
            var message = $"Option does not require a variable.";
            ReportError(span, message);
        }

        internal void ReportMissingVariable(TextSpan span)
        {
            var message = $"Missing variable.";
            ReportError(span, message);
        }

        internal void ReportUnterminatedPipeline(TextSpan span)
        {
            var message = $"Unterminated pipeline, expected command.";
            ReportError(span, message);
        }

        internal void ReportMultipleOptionsNotAllowed(TextSpan span)
        {
            var message = $"Multiple calls to this option are not allowed.";
            ReportError(span, message);
        }

        internal void ReportCommandHasRequiredVariable(TextSpan span)
        {
            var message = $"Command requires a variable.";
            ReportError(span, message);
        }

        internal void ReportCommandIsNotACommandWithVariable(TextSpan span)
        {
            var message = $"Command does not accept variables.";
            ReportError(span, message);
        }
    }
}
