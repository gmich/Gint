namespace Gint.SyntaxHighlighting
{
    internal class CommandTextChangedEventArgs
    {
        public CommandTextChangedEventArgs(string previous, string current)
        {
            Previous = previous;
            Current = current;
        }

        public string Previous { get; }
        public string Current { get; }
    }
}
