using System;

namespace Gint.Markup.Format
{
    internal abstract class ForegroundConsoleColorFormat : IConsoleMarkupFormat
    {
        public abstract string Tag { get; }

        protected abstract ConsoleColor Color { get; }

        private ConsoleColor previousColor;

        public void Apply(string variable)
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
        }
        public void Remove()
        {
            Console.ForegroundColor = previousColor;
        }
    }

}
