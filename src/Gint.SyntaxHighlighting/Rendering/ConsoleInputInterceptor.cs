using System;

namespace Gint.SyntaxHighlighting
{
    internal class ConsoleInputInterceptor
    {
        public ConsoleKeyInfo GetNextKey()
        {
            Console.CursorVisible = true;
            var key = Console.ReadKey(intercept: true);
            Console.CursorVisible = false;
            return key;
        }
    }
}
