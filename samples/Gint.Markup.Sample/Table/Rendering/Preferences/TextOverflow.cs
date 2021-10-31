namespace Gint.Markup.Sample
{
    public class TextOverflow
    {
        public int? MaxCellsPerColumn { get; set; } = null;
        public TextOverflowOption Overflow { get; set; } = TextOverflowOption.ChangeLine;

    }

    public enum TextOverflowOption
    {
        ChangeLine,
        Ellipsis,
        Clip,
        Render
    }

}
