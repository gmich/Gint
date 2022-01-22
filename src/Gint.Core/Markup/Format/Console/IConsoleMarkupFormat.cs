namespace Gint.Markup.Format
{
    public interface IConsoleMarkupFormat
    {
        string Tag { get; }
        void Apply(string variable);
        RenderArea GetAreaEstimate(string variable);
        void Remove();
    }
}
