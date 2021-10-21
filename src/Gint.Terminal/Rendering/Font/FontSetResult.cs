namespace Gint.Terminal
{
    public class FontSetResult
    {
        public FontSetResult(FontInfo before, FontInfo set, FontInfo after)
        {
            Before = before;
            FontToSet = set;
            After = after;
        }

        public FontInfo Before { get; }
        public FontInfo FontToSet { get; }
        public FontInfo After { get; }
    }
}
