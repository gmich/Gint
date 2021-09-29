using System;

namespace Gint.Markup.Format
{
    internal class ForegroundYellow : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.yellow";
        protected override ConsoleColor Color => ConsoleColor.Yellow;
    }
}
