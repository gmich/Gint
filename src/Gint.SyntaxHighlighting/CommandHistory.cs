using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    public class CommandHistory : Stack<string>
    {
        private readonly int limit;

        public CommandHistory(int Limit)
            : base()
        {
            limit = Limit;
        }

        public new void Push(string history)
        {
            if (Count >= limit)
            {
                Pop();
            }
            Push(history);
        }
    }
}
