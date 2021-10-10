﻿namespace Gint.SyntaxHighlighting
{
    public class ConsoleInputOptions
    {
        public bool SuggestionsEnabled { get; init; } = true;
        public bool DisplayErrorCells { get; init; } = false;
        public bool DisplayDiagnostics { get; init; } = true;

        public int MaxSuggestionsPerRow { get; init; } = 6;

        public string Prompt { get; init; } = " » ";

        public ICommandHistoryRepository CommandHistoryRepository { get; init; } = new NoopHistoryRepository();

        public CommandRegistry Registry { get; init; } = CommandRegistry.Empty;

        public static ConsoleInputOptions Default => new ConsoleInputOptions();
    }
}