namespace Gint.Terminal
{
    public class TerminalOptions
    {
        public bool SuggestionsEnabled { get; init; } = true;
        public bool DisplayErrorCells { get; init; } = false;

        /// <summary>
        /// Shows parsing and lexing diagnostics.
        /// 
        /// Can also be toggled with the key combination: control + shift + d
        /// </summary>
        public bool DisplayDiagnostics { get; init; } = true;
        public int MaxSuggestionsPerRow { get; init; } = 5;
        public string Prompt { get; init; } = " » ";
        public TerminalTheme Theme { get; init; } = TerminalTheme.Default;

        public ICommandHistoryRepository CommandHistoryRepository { get; init; } = new NoopHistoryRepository();
        internal CommandRegistry Registry { get; set; }

        /// <summary>
        /// Tries to set the console font by name, and set the size.
        /// If the size is not specified, the size remains the same.
        /// Throws Win32Exception
        /// </summary>
        public FontSetResult SetFont(string font, short size = 0) 
        {
            return FontHelper.SetCurrentFont(font, size);
        }

        public static TerminalOptions Default => new TerminalOptions();
    }
}
