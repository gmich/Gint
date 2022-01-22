namespace Gint.Tables
{
    internal class SquareTable : TableStyleRenderer
    {

        internal SquareTable() : base(new SquareTableBorderStyle(), new SquareTableDividerStyle(), new SquareContentConnectorStyle(), new SquareHeaderConnectorStyle())
        {
        }
    }


    partial class TableStyleRenderer
    {
        public static SquareTable Square { get; } = new SquareTable();
    }
}
