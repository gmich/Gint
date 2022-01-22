namespace Gint.Markup.Format
{
    public record RenderArea (int Width, int Height)
    {
        public static RenderArea Empty => new RenderArea(0, 0);
    }
}
