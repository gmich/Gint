namespace Gint.Markup
{
    public class NullAtStartOfFormat : MarkdownDiagnostic
    {
        internal NullAtStartOfFormat(DiagnosticSeverity severity, TextSpan location, string message) : base(severity, location, message)
        {
        }

    }
}
