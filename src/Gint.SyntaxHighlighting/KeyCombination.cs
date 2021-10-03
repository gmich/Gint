using System;
using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    public class KeyCombination : Queue<ConsoleModifiers>
    {
        private readonly int limit;

        public KeyCombination(int Limit)
            : base()
        {
            limit = Limit;
        }

        public new void Enqueue(ConsoleModifiers modifier)
        {
            if (Count >= limit)
            {
                Dequeue();
            }
            Enqueue(modifier);
        }
    }
}
