using System;

namespace Gint.Markup.Format
{
    internal class DateFormat : IConsoleMarkupFormat
    {
        public string Tag { get; } = "date";

        public void Apply(string variable)
        {
            Console.Write(DateTime.UtcNow.ToString(variable));
        }

        public void Remove()
        {
        }
    }

}
