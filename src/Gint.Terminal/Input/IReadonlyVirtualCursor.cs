namespace Gint.Terminal
{
    internal interface IReadonlyVirtualCursor
    {
        public int Index { get; }
        public int IndexWithPrompt { get; }
    }
}
