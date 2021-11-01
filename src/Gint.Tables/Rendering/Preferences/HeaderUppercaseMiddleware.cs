
namespace Gint.Tables
{
    internal class HeaderUppercaseMiddleware : ITableRenderMiddleware
    {
        private HeaderUppercaseMiddleware() { }

        public static HeaderUppercaseMiddleware Instance { get; } = new HeaderUppercaseMiddleware();

        public void PostWrite(string text, TableSection section)
        {
        }

        public string PreWrite(string text, TableSection section)
        {
            if (section == TableSection.HeaderColumn)
                return text.ToUpperInvariant();
            else
                return text;
        }
    }

}
