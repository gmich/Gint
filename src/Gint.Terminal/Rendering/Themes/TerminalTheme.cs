using System;

namespace Gint.Terminal
{
    public class TerminalTheme
    {
        public ThemeColor Prompt { get; set; }
        public ThemeColor Command { get; set; }
        public ThemeColor CommandWithVariable { get; set; }
        public ThemeColor Option { get; set; }
        public ThemeColor OptionWithVariable { get; set; }
        public ThemeColor Whitespace { get; set; }
        public ThemeColor Quotes { get; set; }
        public ThemeColor Pipe { get; set; }
        public ThemeColor Keyword { get; set; }
        public ThemeColor ErrorCell { get; set; }

        public ThemeColor DiagnosticsFrame { get; set; }
        public ThemeColor DiagnosticsCode { get; set; }
        public ThemeColor DiagnosticsMessage { get; set; }

        public ThemeColor Suggestion { get; set; }
        public ThemeColor SuggestionFocus { get; set; }

        public static TerminalTheme Default => new TerminalTheme
        {
            Prompt = new ThemeColor(foreground: ConsoleColor.DarkGreen),
            Command = new ThemeColor(foreground: ConsoleColor.Green),
            CommandWithVariable = new ThemeColor(foreground: ConsoleColor.Green),
            Option = new ThemeColor(foreground: ConsoleColor.Yellow),
            OptionWithVariable = new ThemeColor(foreground: ConsoleColor.Yellow),
            Keyword = new ThemeColor(foreground: ConsoleColor.White),
            Whitespace = new ThemeColor(foreground: ConsoleColor.White),
            Quotes = new ThemeColor(foreground: ConsoleColor.Magenta),
            Pipe = new ThemeColor(foreground: ConsoleColor.Magenta),

            ErrorCell = new ThemeColor(foreground: ConsoleColor.Red),
            DiagnosticsFrame = new ThemeColor(foreground: ConsoleColor.DarkYellow),
            DiagnosticsCode = new ThemeColor(foreground: ConsoleColor.Cyan),
            DiagnosticsMessage = new ThemeColor(foreground: ConsoleColor.DarkGray),

            Suggestion = new ThemeColor(foreground: ConsoleColor.Gray),
            SuggestionFocus = new ThemeColor(foreground: ConsoleColor.Black, background: ConsoleColor.White),
        };
    }


}