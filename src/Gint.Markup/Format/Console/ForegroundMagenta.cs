using System;

namespace Gint.Markup.Format
{
    internal class ForegroundMagenta : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.magenta";
        protected override ConsoleColor Color => ConsoleColor.Magenta;
    }

}
