using System;

namespace Gint.Markup.Format
{
    internal class BackgroundConsoleColorFormat : ConsoleColorFormat
    {
        public override string Tag => "bg";

        public override void Apply(string variable)
        {
            previousColor = Console.BackgroundColor;
            Console.BackgroundColor = GetColor(variable);
        }

        public override void Remove()
        {
            Console.BackgroundColor = previousColor;
        }
    }
}
