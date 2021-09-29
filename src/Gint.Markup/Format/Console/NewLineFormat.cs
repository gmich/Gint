using System;

namespace Gint.Markup.Format
{
    internal class NewLineFormat : IConsoleMarkupFormat
    {
        public string Tag { get; } = "br";

        public void Apply(string variable)
        {
            Console.WriteLine();
        }

        public void Remove()
        {
        }
    }

}
