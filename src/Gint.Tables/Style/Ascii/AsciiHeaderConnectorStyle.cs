namespace Gint.Tables
{
    public class AsciiHeaderConnectorStyle : IHeaderConnectorStyle
    {
        public char Get(TableConnectorPart connector)
        {
            return connector switch
            {
                TableConnectorPart.Straight => '-',
                TableConnectorPart.Cross => '+',
                TableConnectorPart.Top => '┬',
                TableConnectorPart.Left => '├',
                TableConnectorPart.Right => '┤',
                TableConnectorPart.Bottom => '┴',
                TableConnectorPart.BottomLeft => '└',
                TableConnectorPart.BottomRight => '┘',
                TableConnectorPart.TopLeft => '┌',
                TableConnectorPart.TopRight => '┐',
                TableConnectorPart.Blank => ' ',
                _ => throw new System.ArgumentException($"Unknown connector style {connector}"),
            };
        }
    }
}