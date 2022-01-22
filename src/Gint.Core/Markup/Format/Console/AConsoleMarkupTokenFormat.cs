namespace Gint.Markup.Format
{
    public abstract class  AConsoleMarkupTokenFormat : IConsoleMarkupFormat
    {
        public abstract string Tag { get; }
        public abstract void Apply(string variable);
        public abstract RenderArea GetAreaEstimate(string variable);

        public void Remove() { }
    }

}
