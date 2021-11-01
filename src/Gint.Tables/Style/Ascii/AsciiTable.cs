namespace Gint.Tables
{
    public class AsciiTable : TableStyle
    {
        internal AsciiTable() : base(new AsciiTableBorderStyle(), new AsciiTableDividerStyle(), new AsciiContentConnectorStyle(), new AsciiHeaderConnectorStyle())
        {
        }
    }

    partial class TableStyle
    {
        public static AsciiTable Ascii { get; } = new AsciiTable();
    }
}
