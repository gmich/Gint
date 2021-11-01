namespace Gint.Tables
{
    public class SquareTable : TableStyle
    {

        internal SquareTable() : base(new SquareTableBorderStyle(), new SquareTableDividerStyle(), new SquareContentConnectorStyle(), new SquareHeaderConnectorStyle())
        {
        }
    }


    partial class TableStyle
    {
        public static SquareTable Square { get; } = new SquareTable();
    }
}
