using System;
using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    internal class SuggestionSelector
    {
        private readonly List<SuggestionCollection> suggestions;

        public SuggestionSelector(List<SuggestionCollection> suggestions)
        {
            this.suggestions = suggestions;
        }

        public int Column { get; private set; }
        public int Row { get; private set; }

        public event EventHandler OnCursorExit;

        public void MoveUp()
        {
            if (Row == 0)
            {
                OnCursorExit?.Invoke(this, EventArgs.Empty);
                return;
            }
            Row--;
        }

        private int ColumnMax => suggestions[Row].Count - 1;
        private int RowMax => suggestions.Count - 1;

        public void MoveDown()
        {
            Row = Math.Min(Row + 1, RowMax);
            Column = Math.Min(Column, ColumnMax);
        }

        public void MoveLeft()
        {
            Column = Math.Max(Column - 1, 0);
        }

        public void MoveRight()
        {
            Column = Math.Min(Column + 1, ColumnMax);
        }



    }
}
