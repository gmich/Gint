using System;
using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    public class CommandHistory : List<string>
    {
        private readonly int limit;
        private int lastLookup = 0;

        public CommandHistory(int limit)
            : base()
        {
            this.limit = Math.Max(1, limit);
        }

        private void InternalAdd(string history)
        {
            if (Count >= limit)
            {
                RemoveAt(0);
            }
            Add(history);
            lastLookup = Count;
        }

        public void Record(string history)
        {
           InternalAdd(history);
        }

        internal bool GetPrevious(out string command)
        {
            if (lastLookup > 0)
            {
                lastLookup--;
                command = this[lastLookup];
                return true;
            }
            command = default;
            return false;
        }

        internal bool GetNext(out string command)
        {
            if (Count > lastLookup + 1)
            {
                lastLookup++;
                command = this[lastLookup];
                return true;
            }
            command = string.Empty;
            return true;
        }
    }
}
