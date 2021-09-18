namespace Gint.Markup
{
    public class MarkdownDiagnostic
    {
        internal MarkdownDiagnostic(DiagnosticSeverity severity, TextSpan location, string message)
        {
            Severity = severity;
            Location = location;
            Message = message;
        }

        public DiagnosticSeverity Severity { get; }
        public TextSpan Location { get; }
        public string Message { get; }

        public override string ToString() => $"{Severity}: {Message}";
    }
 

    public enum DiagnosticSeverity
    {
        Warning,
        Error
    }
}
