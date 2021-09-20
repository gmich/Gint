namespace Gint.Markup
{
    public class EndTagWithVariable : MarkdownDiagnostic
    {
        internal EndTagWithVariable(DiagnosticSeverity severity, TextSpan location, string message) : base(severity, location, message)
        {
        }

    }
}
