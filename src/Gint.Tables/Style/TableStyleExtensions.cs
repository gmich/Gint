namespace Gint.Tables
{
    internal static class TableStyleExtensions
    {
        public static TableStyleRenderer GetRenderer(this TableStyle style)
        {
            switch (style)
            {
                case TableStyle.Square:
                    return TableStyleRenderer.Square;
                case TableStyle.Ascii:
                    return TableStyleRenderer.Ascii;
                case TableStyle.OnlyHeaderUnderline:
                    return TableStyleRenderer.OnlyHeaderUnderline;
                default:
                    return TableStyleRenderer.Square;
            }
        }
    }
}