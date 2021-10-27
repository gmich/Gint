namespace Gint.Markup.Sample
{
    public class TableRenderPreferences
    {
        public Alignment DefaultHeaderAlignment { get; } = Alignment.Center;
        public Alignment DefaultContentAlignment { get; } = Alignment.Center;
        public bool TryFitToScreen { get; } = false;
        public int ColumnPaddingLeft { get; } = 3;
        public int ColumnPaddingRight { get; } = 3;
        public TableStyle TableStyle { get; } = new TableStyle();
    }

}
