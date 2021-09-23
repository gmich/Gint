using System;

namespace Gint.Markup.Format
{
    internal class ForegroundRed : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.red";
        protected override ConsoleColor Color => ConsoleColor.Red;
    }

}
