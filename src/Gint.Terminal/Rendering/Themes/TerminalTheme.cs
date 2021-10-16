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
            Prompt = new ThemeColor(ConsoleColor.DarkGreen),
            Command = new ThemeColor(ConsoleColor.Green),
            CommandWithVariable =  new ThemeColor(ConsoleColor.Green),
            Option = new ThemeColor(ConsoleColor.Yellow),
            OptionWithVariable = new ThemeColor(ConsoleColor.Yellow),
            Keyword = new ThemeColor(ConsoleColor.White),
            Whitespace = new ThemeColor(ConsoleColor.White),
            Quotes = new ThemeColor(ConsoleColor.Magenta),
            Pipe = new ThemeColor(ConsoleColor.Magenta),

            ErrorCell = new ThemeColor(ConsoleColor.Red),
            DiagnosticsFrame = new ThemeColor(ConsoleColor.DarkYellow),
            DiagnosticsCode = new ThemeColor(ConsoleColor.Cyan),
            DiagnosticsMessage = new ThemeColor(ConsoleColor.DarkGray),

            Suggestion = new ThemeColor(ConsoleColor.Gray),
            SuggestionFocus = new ThemeColor(ConsoleColor.Black, ConsoleColor.White),
        };
    }


}