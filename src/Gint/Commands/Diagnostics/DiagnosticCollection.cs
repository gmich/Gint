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

        private void ReportError(string errorCode, TextSpan location, string message)
        {
            var diagnostic = Diagnostic.Error(errorCode, location, message);
            diagnostics.Add(diagnostic);
        }

        private void ReportWarning(string errorCode, TextSpan location, string message)
        {
            var diagnostic = Diagnostic.Warning(errorCode, location, message);
            diagnostics.Add(diagnostic);
        }

        internal void ReportUnterminatedString(TextSpan span)
        {
            var message = "Unterminated string literal.";
            ReportError(DiagnosticsErrorCode.UnterminatedString, span, message);
        }

        internal void ReportUnexpectedToken(TextSpan span, CommandTokenKind actualKind, CommandTokenKind expectedKind)
        {
            var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            ReportError(DiagnosticsErrorCode.UnexpectedToken, span, message);
        }

        internal void ReportCommandUnknown(TextSpan span, string cmd)
        {
            var errorCode = string.IsNullOrEmpty(cmd) ? DiagnosticsErrorCode.NullCommand : DiagnosticsErrorCode.CommandUnknown;
            var message = $"Unknown command <{cmd}>.";
            ReportWarning(errorCode, span, message);
        }

        internal void ReportOptionUnknown(TextSpan span, string option)
        {
            var errorCode = string.IsNullOrEmpty(option) ? DiagnosticsErrorCode.NullOption : DiagnosticsErrorCode.OptionUnknown;
            var message = $"Unknown option <{option}>.";
            ReportWarning(errorCode, span, message);
        }

        internal void ReportUnecessaryVariable(TextSpan span)
        {
            var message = $"Option does not require a variable.";
            ReportError(DiagnosticsErrorCode.UnecessaryVariable, span, message);
        }

        internal void ReportMissingVariable(TextSpan span)
        {
            var message = $"Missing variable.";
            ReportError(DiagnosticsErrorCode.MissingVariable, span, message);
        }

        internal void ReportUnterminatedPipeline(TextSpan span)
        {
            var message = $"Unterminated pipeline, expected command.";
            ReportError(DiagnosticsErrorCode.UnterminatedPipeline, span, message);
        }

        internal void ReportMultipleOptionsNotAllowed(TextSpan span)
        {
            var message = $"Multiple calls to this option are not allowed.";
            ReportWarning(DiagnosticsErrorCode.MultipleOptionsNotAllowed, span, message);
        }

        internal void ReportCommandHasRequiredVariable(TextSpan span)
        {
            var message = $"Command requires a variable.";
            ReportError(DiagnosticsErrorCode.CommandHasRequiredVariable, span, message);
        }

        internal void ReportCommandIsNotACommandWithVariable(TextSpan span)
        {
            var message = $"Command does not accept variables.";
            ReportError(DiagnosticsErrorCode.CommandIsNotACommandWithVariable, span, message);
        }

        internal void ReportMissingCommandToPipe(TextSpan span)
        {
            var message = $"Missing command before pipe.";
            ReportError(DiagnosticsErrorCode.MissingCommandToPipe, span, message);
        }
    }
}
