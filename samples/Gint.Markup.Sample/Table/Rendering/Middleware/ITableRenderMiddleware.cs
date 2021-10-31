namespace Gint.Markup.Sample
{
    public interface ITableRenderMiddleware
    {
        string PreWrite(string text, TableSection section);
        void PostWrite(string text, TableSection section);
    }

}
