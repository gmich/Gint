﻿using System.Linq;

namespace Gint.Markup.Sample
{
    internal class TableRenderParameters
    {
        public int CellSize { get; }
        public int PaddingLeft { get; }
        public int PaddingRight { get; }
        public int TotalWidthWithoutMargin { get; }
        public int TotalColumns { get; }
        public int ColumnDividerWidth { get; } = 1;
        public int TotalRowCells => TotalColumns * (CellSize + PaddingLeft + PaddingRight + ColumnDividerWidth) + ColumnDividerWidth;

        public TableRenderParameters(Table table, TableRenderPreferences preferences)
        {
            TotalColumns = table.Header.Row.Columns.Count();
            PaddingLeft = preferences.ColumnPaddingLeft;
            PaddingRight = preferences.ColumnPaddingRight;
            CellSize = preferences.TryFitToScreen ? GetCellSizeFitToScreen() : GetLargestColumnSize(table);
            TotalWidthWithoutMargin = CellSize + PaddingLeft + PaddingRight;
        }

        private int GetLargestColumnSize(Table table)
        {
            return table.IterateColumns.Max(c => c.Content.Length / c.SpansOverColumns);
        }

        private int GetCellSizeFitToScreen()
        {
            var offSet = (PaddingLeft + PaddingRight + ColumnDividerWidth) * TotalColumns + ColumnDividerWidth;
            var cellSize = (System.Console.BufferWidth - offSet) / TotalColumns;
            return cellSize;
        }
    }

    public class TableStyle
    {
        public TableBorder TableBorder { get; } = new TableBorder();
        public TableInside TableInsight { get; } = new TableInside();
    }

    public class TableBorder
    {
        public char TopLeft { get; } = '┌';
        public char Top { get; } = '─';
        public char TopRight { get; } = '┐';

        public char Left { get; } = '│';
        public char Right { get; } = '│';

        public char BottomLeft { get; } = '└';
        public char Bottom { get; } = '─';
        public char BottomRight { get; } = '┘';
    }

    public class TableInside
    {
        public char HeaderColumnDivider { get; } = '│';
        public char HeaderRowDivider { get; } = '─';
        public char HeaderConnector { get; } = '─';
        public char ContentColumnDivider { get; } = '│';
        public char ContentRowDivider { get; } = '─';
        public char ContentConnector { get; } = '─'; //'┼';
    }

}
