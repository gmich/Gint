namespace Gint.Markup.Format
{
    public interface IMarkupFormat
    {
        string Tag { get; }
        void Apply();
        void Remove();
    }

}
