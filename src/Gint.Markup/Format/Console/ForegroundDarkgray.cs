using System;

namespace Gint.Markup.Format
{
    internal class ForegroundDarkgray : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.darkgray";
        protected override ConsoleColor Color => ConsoleColor.DarkGray;
    }
}
