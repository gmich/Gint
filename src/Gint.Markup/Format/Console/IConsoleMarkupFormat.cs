namespace Gint.Markup.Format
{
    public interface IConsoleMarkupFormat
    {
        string Tag { get; }
        void Apply(string variable);
        void Remove();
    }

}
