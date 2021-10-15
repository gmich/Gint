using System;

namespace Gint.Terminal
{
    internal class Prompt
    {
        public Prompt(string text)
        {
            Text = text;
        }

        public string Text { get; }
        public int Length => Text.Length;

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(Text);
            Console.ResetColor();
        }
    }
}
