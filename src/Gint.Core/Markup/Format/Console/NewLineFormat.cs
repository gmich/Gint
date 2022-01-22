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

        public RenderArea GetAreaEstimate(string variable) => new RenderArea(0, 1);

        public void Remove()
        {
        }

    }

}
