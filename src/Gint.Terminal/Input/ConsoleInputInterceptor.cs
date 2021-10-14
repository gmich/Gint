using System;

namespace Gint.Terminal
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
