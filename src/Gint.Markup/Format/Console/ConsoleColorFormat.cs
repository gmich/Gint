using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.Markup.Format
{
    internal abstract class ConsoleColorFormat : IConsoleMarkupFormat
    {
        public abstract string Tag { get; }

        protected ConsoleColor previousColor;
        private static Dictionary<string, ConsoleColor> colorMap;

        static ConsoleColorFormat()
        {
            colorMap = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToDictionary(c => c.ToString().ToUpperInvariant(), c => c);
        }

        protected ConsoleColor GetColor(string color)
        {
            var key = color.ToUpperInvariant();
            if (colorMap.ContainsKey(key))
                return colorMap[key];
            else
                return ConsoleColor.White;
        }

        public abstract void Apply(string variable);
        public abstract void Remove();
    }
}
