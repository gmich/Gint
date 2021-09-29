using System;

namespace Gint.Markup.Format
{
    internal class ForegroundCyan : ForegroundConsoleColorFormat
    {
        public override string Tag => "fg.cyan";
        protected override ConsoleColor Color => ConsoleColor.Cyan;
    }
}
