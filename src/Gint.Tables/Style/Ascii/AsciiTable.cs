namespace Gint.Tables
{
    internal class AsciiTable : TableStyleRenderer
    {
        internal AsciiTable() : base(new AsciiTableBorderStyle(), new AsciiTableDividerStyle(), new AsciiContentConnectorStyle(), new AsciiHeaderConnectorStyle())
        {
        }
    }

    partial class TableStyleRenderer
    {
        public static AsciiTable Ascii { get; } = new AsciiTable();
    }
}
