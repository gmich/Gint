using System;

namespace Gint.Markup.Format
{

    internal class BackgroundWhite : BackgroundConsoleColorFormat
    {
        public override string Tag => "bg.white";
        protected override ConsoleColor Color => ConsoleColor.White;
    }

}
