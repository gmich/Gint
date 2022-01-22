namespace Gint.Tables
{
    internal abstract partial class TableStyleRenderer
    {
        public TableStyleRenderer(ITableBorderStyle tableBorder, ITableDividerStyle tablePart, IContentConnectorStyle contentConnector, IHeaderConnectorStyle headerConnector)
        {
            TableBorder = tableBorder;
            TablePart = tablePart;
            ContentConnector = contentConnector;
            HeaderConnector = headerConnector;
        }

        public ITableBorderStyle TableBorder { get; }
        public ITableDividerStyle TablePart { get; }
        public IContentConnectorStyle ContentConnector { get; }
        public IHeaderConnectorStyle HeaderConnector { get; }
    }
}