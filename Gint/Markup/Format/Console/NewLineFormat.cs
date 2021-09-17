using System;

namespace Gint.Markup.Format
{
    internal class NewLineFormat : IMarkupFormat
    {
        public string Tag { get; } = "br";

        public void Apply()
        {
            Console.WriteLine();
        }
        public void Remove()
        {
        }
    }

}
