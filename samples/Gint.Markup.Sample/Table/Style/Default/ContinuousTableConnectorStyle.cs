namespace Gint.Markup.Sample
{
    public class ContinuousTableConnectorStyle : ITableConnectorStyle
    {
        public char Get(TableConnectorPart connector)
        {
            return connector switch
            {
                TableConnectorPart.Straight => '─',
                TableConnectorPart.Cross => '┼',
                TableConnectorPart.Top => '┬',
                TableConnectorPart.Left => '├',
                TableConnectorPart.Right => '┤',
                TableConnectorPart.Bottom => '┴',
                TableConnectorPart.BottomLeft => '└',
                TableConnectorPart.BottomRight => '┘',
                TableConnectorPart.TopLeft => '┌',
                TableConnectorPart.TopRight => '┘',
                _ => throw new System.ArgumentException($"Unknown connector style {connector}"),
            };
        }

    }
}