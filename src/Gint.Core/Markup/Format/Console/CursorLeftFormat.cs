using System;

namespace Gint.Markup.Format
{
    internal class CursorLeftFormat : AConsoleMarkupTokenFormat
    {
        public override string Tag { get; } = "cursor-left";

        public override void Apply(string variable)
        {
            if(int.TryParse(variable, out int pos))
            {
                Console.CursorLeft = pos;
            }
        }

    }
}
