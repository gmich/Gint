namespace Gint.Tables
{
    public class OnlyHeaderUnderlineTable : TableStyle
    {

        internal OnlyHeaderUnderlineTable() : base(new OnlyHeaderUnderlineTableBorderStyle(), new OnlyHeaderUnderlineTableDividerStyle(), new OnlyHeaderUnderlineConnectorStyle(), new OnlyHeaderUnderlineHeaderConnectorStyle())
        {
        }
    }


    partial class TableStyle
    {
        public static OnlyHeaderUnderlineTable OnlyHeaderUnderline { get; } = new OnlyHeaderUnderlineTable();
    }
}
