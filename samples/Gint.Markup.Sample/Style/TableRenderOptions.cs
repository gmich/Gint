namespace Gint.Markup.Sample
{
    public class TableRenderOptions
    {
        public Alignment DefaultHeaderAlignment { get; } = Alignment.Center;
        public Alignment DefaultContentAlignment { get; } = Alignment.Center;

        public int CellSize { get; }
        public int PaddingLeft { get; }
        public int PaddingRight { get; }
        public int TotalWidthWithoutMargin { get; }
        public int TotalColumns { get; }
        public int ColumnDividerWidth { get; } = 1;

        public TableRenderOptions(int cellSize, int totalColumns, int paddingLeft = 2, int paddingRight = 2)
        {
            CellSize = cellSize;
            TotalColumns = totalColumns;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            TotalWidthWithoutMargin = cellSize + paddingLeft + paddingRight;
        }
    }

}
