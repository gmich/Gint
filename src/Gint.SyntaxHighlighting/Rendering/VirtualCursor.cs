using System;

namespace Gint.SyntaxHighlighting
{
    internal class VirtualCursor : IReadonlyVirtualCursor
    {
        public event EventHandler OnPositionChanged;
        public Func<int> RightBound;
        private readonly Prompt prompt;

        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            private set
            {
                var previous = _index;
                _index = value;

                //if (previous != _index)
                {
                    OnPositionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int IndexWithPrompt => Index + prompt.Length;

        public VirtualCursor(Func<int> rightBound, Prompt prompt)
        {
            RightBound = rightBound;
            this.prompt = prompt;
        }

        private void MoveCursor(int steps)
        {
            var previous = Index;
            Index = Math.Clamp(Index + steps, 0, RightBound());
        }

        public void Forward(int steps = 1)
        {
            MoveCursor(steps);
        }

        public void Back(int steps = 1)
        {
            MoveCursor(-steps);
        }

        public void Reset()
        {
            Index = 0;
        }
    }
}
