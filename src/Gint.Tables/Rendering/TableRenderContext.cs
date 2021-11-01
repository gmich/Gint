using System.Linq;

namespace Gint.Tables
{
    internal class TableRenderContext
    {
        public int CellSize { get; }
        public int PaddingLeft { get; }
        public int PaddingRight { get; }
        public int TotalWidthWithoutMargin { get; }
        public int TotalColumns { get; }
        public int ColumnDividerWidth { get; } = 1;
        public int TotalRowCells => TotalColumns * (CellSize + PaddingLeft + PaddingRight + ColumnDividerWidth) + ColumnDividerWidth;

        public TableRenderContext(Table table, TableRenderPreferences preferences)
        {
            TotalColumns = table.IterateFirstRow.Sum(c => c.SpansOverColumns);
            PaddingLeft = preferences.ColumnPaddingLeft;
            PaddingRight = preferences.ColumnPaddingRight;
            CellSize = preferences.TryFitToScreen ? GetCellSizeFitToScreen() : GetLargestColumnSize(table);
            TotalWidthWithoutMargin = CellSize + PaddingLeft + PaddingRight;
        }

        private int GetLargestColumnSize(Table table)
        {
            return table.IterateColumns.Max(c => c.ContentLength / c.SpansOverColumns);
        }

        private int GetCellSizeFitToScreen()
        {
            var offSet = (PaddingLeft + PaddingRight + ColumnDividerWidth) * TotalColumns + ColumnDividerWidth;
            var cellSize = (System.Console.BufferWidth - offSet) / TotalColumns;
            return cellSize;
        }
    }
}