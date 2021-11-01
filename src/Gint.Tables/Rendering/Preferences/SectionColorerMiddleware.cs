using System;

namespace Gint.Tables
{
    internal class SectionColorerMiddleware : ITableRenderMiddleware
    {
        public ConsoleColor Header { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor Content { get; set; } = ConsoleColor.DarkCyan;
        public ConsoleColor Border { get; set; } = ConsoleColor.DarkMagenta;

        public void PostWrite(string text, TableSection section)
        {
            Console.ResetColor();
        }

        public string PreWrite(string text, TableSection section)
        {
            switch (section)
            {
                case TableSection.HeaderColumn:
                    Console.ForegroundColor = Header;
                    break;
                case TableSection.ContentColumn:
                    Console.ForegroundColor = Content;
                    break;
                default:
                    Console.ForegroundColor = Border;
                    break;
            }
            return text;
        }
    }

}
