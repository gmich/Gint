namespace Gint.Tables
{
    internal class OnlyHeaderUnderlineTable : TableStyleRenderer
    {

        internal OnlyHeaderUnderlineTable() : base(new OnlyHeaderUnderlineTableBorderStyle(), new OnlyHeaderUnderlineTableDividerStyle(), new OnlyHeaderUnderlineConnectorStyle(), new OnlyHeaderUnderlineHeaderConnectorStyle())
        {
        }
    }


    internal partial class TableStyleRenderer
    {
        public static OnlyHeaderUnderlineTable OnlyHeaderUnderline { get; } = new OnlyHeaderUnderlineTable();
    }
}
