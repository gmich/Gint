using System;

namespace Gint.Markup.Format
{
    internal class ForegroundDarkYellow : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.darkyellow";
        protected override ConsoleColor Color => ConsoleColor.DarkYellow;
    }
}
