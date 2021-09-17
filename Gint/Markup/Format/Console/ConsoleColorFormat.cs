using System;

namespace Gint.Markup.Format
{
    internal abstract class ForegroundConsoleColorFormat : IMarkupFormat
    {
        public abstract string Tag { get; }

        protected abstract ConsoleColor Color { get; }

        private ConsoleColor previousColor;

        public void Apply()
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
        }
        public void Remove()
        {
            Console.ForegroundColor = previousColor;
        }
    }

    internal abstract class BackgroundConsoleColorFormat : IMarkupFormat
    {
        public abstract string Tag { get; }

        protected abstract ConsoleColor Color { get; }

        private ConsoleColor previousColor;

        public void Apply()
        {
            previousColor = Console.BackgroundColor;
            Console.BackgroundColor = Color;
        }
        public void Remove()
        {
            Console.BackgroundColor = previousColor;
        }
    }


    internal class ForegroundRed : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg:red";
        protected override ConsoleColor Color => ConsoleColor.Red;
    }

    internal class ForegroundGreen : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg:green";
        protected override ConsoleColor Color => ConsoleColor.Green;
    }

    internal class BackgroundWhite : BackgroundConsoleColorFormat
    {
        public override string Tag => "bg:white";
        protected override ConsoleColor Color => ConsoleColor.White;
    }

}
