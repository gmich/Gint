using System;

namespace Gint.Markup.Format
{
    internal class ForegroundGreen : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.green";
        protected override ConsoleColor Color => ConsoleColor.Green;
    }

}
