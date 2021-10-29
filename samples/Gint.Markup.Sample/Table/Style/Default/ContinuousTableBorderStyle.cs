namespace Gint.Markup.Sample
{
    public class ContinuousTableBorderStyle : ITableBorderStyle
    {
        public char Get(TableBorderPart border)
        {
            return border switch
            {
                TableBorderPart.TopLeft => '┌',
                TableBorderPart.Top => '─',
                TableBorderPart.TopRight => '┐',
                TableBorderPart.Left => '│',
                TableBorderPart.Right => '│',
                TableBorderPart.BottomLeft => '└',
                TableBorderPart.Bottom => '─',
                TableBorderPart.BottomRight => '┘',
                _ => throw new System.ArgumentException($"Unknown border style {border}"),
            };
        }
    }
}