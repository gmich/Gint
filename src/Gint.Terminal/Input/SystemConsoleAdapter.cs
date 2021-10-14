using System;

namespace Gint.Terminal
{
    internal class SystemConsoleAdapter
    {
        private readonly ConsoleVirtualBufferHandler consoleWindowResizer;
        private readonly IReadonlyVirtualCursor virtualCursor;

        public SystemConsoleAdapter(Prompt prompt, ConsoleVirtualBufferHandler consoleWindowResizer, IReadonlyVirtualCursor virtualCursor)
        {
            this.consoleWindowResizer = consoleWindowResizer;
            this.virtualCursor = virtualCursor;
        }

        public event EventHandler OnInputCleared;

        public void ClearConsoleInput(int characters)
        {
            var totalLines = characters / Console.BufferWidth;
            totalLines += 1;

            var total = totalLines * Console.WindowWidth;
            SetConsoleCursorToInputStart();

            var cleanup = new string(' ', total);
            Console.Write(cleanup);
            SetConsoleCursorToInputStart();
            OnInputCleared?.Invoke(this, EventArgs.Empty);
        }

        public void NewLine()
        {
            Console.WriteLine();
        }

        public void AdjustToVirtualCursor()
        {
            int line = (virtualCursor.IndexWithPrompt / Console.BufferWidth);

            var shouldBe = consoleWindowResizer.InputCursorTop + line;
            Console.CursorTop = shouldBe;
            Console.CursorLeft = virtualCursor.IndexWithPrompt % Console.BufferWidth;
        }

        public void SetConsoleCursorToInputStart()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = consoleWindowResizer.InputCursorTop;
        }
    }
}
