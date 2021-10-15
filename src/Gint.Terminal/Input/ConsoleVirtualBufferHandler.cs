using System;

namespace Gint.Terminal
{
    internal class ConsoleVirtualBufferHandler
    {
        private readonly CommandText commandText;
        private readonly IReadonlyVirtualCursor virtualCursor;
        private int beforeReadKeyTop;
        private int beforeReadKeyBufferWidth;
        private int linesBeforeRead;
        private bool inputIsEmpty;

        public int InputCursorTop { get; private set; }

        public int InputCursorTopOnLastRenderCell { get; private set; }

        public ConsoleVirtualBufferHandler(IReadonlyVirtualCursor virtualCursor,CommandText commandText)
        {
            this.virtualCursor = virtualCursor;
            this.commandText = commandText;
            InputCursorTop = Console.CursorTop;
            InputCursorTopOnLastRenderCell = Console.CursorTop;
        }

        public void RecordLastCursorLine()
        {
            InputCursorTopOnLastRenderCell = Console.CursorTop;
        }

        public void RecordInputTop()
        {
            InputCursorTop = Console.CursorTop;
        }

        public int GetTotalCharactersInVirtualBuffer()
        {
            return ((InputCursorTopOnLastRenderCell - InputCursorTop) + 1) * Console.BufferWidth;
        }

        public void RecordBufferState()
        {
            inputIsEmpty = string.IsNullOrEmpty(commandText.Value);
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

                //if there is no input, for some reason, when u resize the console the cursor jumps at the end of the last written.
                var cursorTop = inputIsEmpty ? beforeReadKeyTop : Console.CursorTop;

                var topDifference = (cursorTop - beforeReadKeyTop);
                if (linesAfterRead != linesBeforeRead)
                {
                    topDifference += (linesBeforeRead - linesAfterRead);
                }
                InputCursorTop += topDifference;
                InputCursorTopOnLastRenderCell += topDifference;
            }
        }
    }
}
