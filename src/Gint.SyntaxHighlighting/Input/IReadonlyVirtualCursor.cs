namespace Gint.SyntaxHighlighting
{
    internal interface IReadonlyVirtualCursor
    {
        public int Index { get; }
        public int IndexWithPrompt { get; }
    }
}
