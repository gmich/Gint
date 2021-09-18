namespace Gint.Markup
{
    public class UnterminatedFormat : MarkdownDiagnostic
    {
        internal UnterminatedFormat(DiagnosticSeverity severity, TextSpan location, string message, string format) : base(severity, location, message)
        {
            Format = format;
        }

        public string Format { get; }
    }
}
