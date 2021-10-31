namespace Gint.Markup.Sample
{
    public class SquareTable : TableStyle
    {
        public static SquareTable Style { get; } = new SquareTable();

        internal SquareTable() : base(new SquareTableBorderStyle(), new SquareTableDividerStyle(), new SquareContentConnectorStyle(), new SquareHeaderConnectorStyle())
        {
        }
    }
}
