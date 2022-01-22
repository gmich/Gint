namespace Gint.Markup.Format
{
    public class NoopFormat : IConsoleMarkupFormat
    {
        public string Tag => string.Empty;
        public void Apply(string variable) { }
        public RenderArea GetAreaEstimate(string variable) => new RenderArea(0, 0);

        public void Remove() { }
    }

}
