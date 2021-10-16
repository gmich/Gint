using System;

namespace Gint.Terminal
{
    public struct ThemeColor
    {
        public ThemeColor(ConsoleColor foreground = ConsoleColor.Gray, ConsoleColor background = ConsoleColor.Black)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }

        internal void Apply()
        {
            Console.ForegroundColor = Foreground;
            Console.BackgroundColor = Background;
        }

        public static ThemeColor Default { get; } = new ThemeColor();
    }
}