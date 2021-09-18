namespace Gint.Markup
{
    public class MissingTag : MarkdownDiagnostic
    {
        public enum TagPosition
        {
            Start,
            End,
        }

        internal MissingTag(DiagnosticSeverity severity, TextSpan location, string message, string tag, TagPosition position) : base(severity, location, message)
        {
            Tag = tag;
            Position = position;
        }

        public TagPosition Position { get; }
        public string Tag { get; }
    }
}
