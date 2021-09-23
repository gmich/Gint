namespace Gint.Markup.Format
{
    public class NoopFormat : IConsoleMarkupFormat
    {
        public string Tag => string.Empty;
        public void Apply(string variable) { }
        public void Remove() { }
    }

}
