using System;

namespace Gint.Markup.Sample
{
    public class SectionColorerMiddleware : ITableRenderMiddleware
    {
        public void PostWrite(string text, TableSection section)
        {
            Console.ResetColor();
        }

        public string PreWrite(string text, TableSection section)
        {
            if (section == TableSection.HeaderColumn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                return text.ToUpper();
            }
            else if (section == TableSection.ContentColumn)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            return text;
        }
    }

}
