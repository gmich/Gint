namespace Gint.Terminal
{
    public class TerminalOptions
    {
        public bool SuggestionsEnabled { get; init; } = true;
        public bool DisplayErrorCells { get; init; } = false;
        public bool DisplayDiagnostics { get; init; } = true;
        public int MaxSuggestionsPerRow { get; init; } = 5;
        public string Prompt { get; init; } = " » ";
        public TerminalTheme Theme { get; init; } = TerminalTheme.Default;

        public ICommandHistoryRepository CommandHistoryRepository { get; init; } = new NoopHistoryRepository();
        public CommandRegistry Registry { get; init; } = CommandRegistry.Empty;


        public static TerminalOptions Default => new TerminalOptions();
    }
}
