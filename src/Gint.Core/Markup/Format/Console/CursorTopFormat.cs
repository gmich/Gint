using System;

namespace Gint.Markup.Format
{
    internal class CursorTopFormat : AConsoleMarkupTokenFormat
    {
        public override string Tag { get; } = "cursor-top";

        public override void Apply(string variable)
        {
            if (int.TryParse(variable, out int pos))
            {
                Console.CursorTop = pos;
            }
        }

    }
}
