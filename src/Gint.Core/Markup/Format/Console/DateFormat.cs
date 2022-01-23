using System;

namespace Gint.Markup.Format
{
    internal class DateFormat : AConsoleMarkupTokenFormat
    {
        public override string Tag { get; } = "date";

        public override void Apply(string variable)
        {
            Console.Write(DateTime.UtcNow.ToString(variable));
        }
    }
}
