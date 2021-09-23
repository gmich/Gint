using System;

namespace Gint.Markup.Format
{
    internal abstract class BackgroundConsoleColorFormat : IConsoleMarkupFormat
    {
        public abstract string Tag { get; }

        protected abstract ConsoleColor Color { get; }

        private ConsoleColor previousColor;

        public void Apply(string variable)
        {
            previousColor = Console.BackgroundColor;
            Console.BackgroundColor = Color;
        }
        public void Remove()
        {
            Console.BackgroundColor = previousColor;
        }
    }

}
