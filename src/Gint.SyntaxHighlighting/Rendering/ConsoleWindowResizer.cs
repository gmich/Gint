using System;

namespace Gint.SyntaxHighlighting
{
    internal class ConsoleWindowResizer
    {
        private int beforeReadKeyTop;
        private int beforeReadKeyBufferWidth;
        private int linesBeforeRead;
        private readonly IReadonlyVirtualCursor virtualCursor;

        public int InputCursorTop { get; private set; }

        public ConsoleWindowResizer(IReadonlyVirtualCursor virtualCursor)
        {
            this.virtualCursor = virtualCursor;
            InputCursorTop = Console.CursorTop;
        }

        public void RecordBufferState()
        {
            //get buffer and console top location
            beforeReadKeyTop = Console.CursorTop;
            beforeReadKeyBufferWidth = Console.BufferWidth;
            linesBeforeRead = virtualCursor.IndexWithPrompt / Console.BufferWidth;
        }

        public void AdjustIfNeeded()
        {
            //compare and reset
            if (beforeReadKeyBufferWidth != Console.BufferWidth)
            {
                var linesAfterRead = virtualCursor.IndexWithPrompt / Console.BufferWidth;
                var topDifference = (Console.CursorTop - beforeReadKeyTop);
                if (linesAfterRead != linesBeforeRead)
                {
                    topDifference += (linesBeforeRead - linesAfterRead);
                }
                InputCursorTop += topDifference;
            }
        }
    }
}
