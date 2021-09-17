namespace Gint.Markup.Format
{
    public class NoopFormat : IMarkupFormat
    {
        public string Tag => string.Empty;
        public void Apply() { }
        public void Remove() { }
    }

}
