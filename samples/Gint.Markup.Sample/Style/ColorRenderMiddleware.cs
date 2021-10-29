using System;

namespace Gint.Markup.Sample
{
    public class ColorRenderMiddleware : ITableRenderMiddleware
    {
        public void PostWrite(string text, TableSection section)
        {
            Console.ResetColor();
        }

        public string PreWrite(string text, TableSection section)
        {
            if (section == TableSection.HeaderColumn)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (section == TableSection.ContentColumn)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            return text;
        }
    }

}
