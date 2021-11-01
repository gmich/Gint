namespace Gint.Tables
{
    public class AsciiTableBorderStyle : ITableBorderStyle
    {
        public char Get(TableBorderPart border)
        {
            return border switch
            {
                TableBorderPart.TopLeft => '┌',
                TableBorderPart.Top => '-',
                TableBorderPart.TopRight => '┐',
                TableBorderPart.Left => '|',
                TableBorderPart.Right => '|',
                TableBorderPart.BottomLeft => '└',
                TableBorderPart.Bottom => '-',
                TableBorderPart.BottomRight => '┘',
                TableBorderPart.TopConnector => '-',
                TableBorderPart.LeftConnector => '|',
                TableBorderPart.RightConnector => '|',
                TableBorderPart.BottomConnector => '-',
                _ => throw new System.ArgumentException($"Unknown border style {border}"),
            };
        }
    }
}