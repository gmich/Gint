using System;

namespace Gint.Terminal
{
    internal class Prompt
    {
        private readonly TerminalTheme theme;

        public Prompt(string text, TerminalTheme theme)
        {
            Text = text;
            this.theme = theme;
        }

        public string Text { get; }

        public int Length => Text.Length;

        public void Print()
        {
            theme.Prompt.Apply();
            Console.Write(Text);
            Console.ResetColor();
        }
    }
}
