namespace Gint.Markup.Sample
{
    public class TableStyle
    {
        public TableStyle(ITableBorderStyle tableBorder, ITableDividerStyle tablePart, ITableConnectorStyle connector)
        {
            TableBorder = tableBorder;
            TablePart = tablePart;
            Connector = connector;
        }

        public ITableBorderStyle TableBorder { get; }
        public ITableDividerStyle TablePart { get; }
        public ITableConnectorStyle Connector { get; }
    }
}