namespace Gint.Markup
{
    public class UnterminatedFormatVariable : MarkdownDiagnostic
    {
        internal UnterminatedFormatVariable(DiagnosticSeverity severity, TextSpan location, string message, string variable) : base(severity, location, message)
        {
            Variable = variable;
        }

        public string Variable { get; }
    }
}
