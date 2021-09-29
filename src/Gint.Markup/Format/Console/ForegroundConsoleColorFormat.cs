using System;

namespace Gint.Markup.Format
{

    internal class ForegroundConsoleColorFormat : ConsoleColorFormat
    {
        public override string Tag => "fg";

        public override void Apply(string variable)
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColor(variable);
        }

        public override void Remove()
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
