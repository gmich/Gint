namespace Gint.Markup.Sample
{
    public class TableRenderPreferences
    {
        public Alignment DefaultHeaderAlignment { get; set; } = Alignment.Center;
        public Alignment DefaultContentAlignment { get; set; } = Alignment.Center;
        public bool TryFitToScreen { get; set; } = true;
        public int ColumnPaddingLeft { get; set; } = 2;
        public int ColumnPaddingRight { get; set; } = 2;
        public TableStyle TableStyle { get; set; } = SquareTable.Style;
        public ITableRenderMiddleware TableRenderMiddleware { get; set; } = new NoopRenderMiddleware();
        public TextOverflow TextOverflow { get; set; } = new TextOverflow();
    }
}
