using System;

namespace Gint.SyntaxHighlighting
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputManager = new ConsoleInputManager();

            while(true)
            {
                inputManager.Run();
            }
        }
    }
}
