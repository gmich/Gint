using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    public class CommandHistory : Stack<string>
    {
        private readonly int limit;

        public CommandHistory(int limit)
            : base()
        {
            this.limit = limit;
        }

        public new void Push(string history)
        {
            if (Count >= limit)
            {
                Pop();
            }
            Push(history);
        }

        public void Record(string history)
        {
            Push(history);
        }
    }
}
