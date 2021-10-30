using System.IO;

namespace Gint.Markup.Sample
{
    public interface ITableRenderer
    {
        void Render(TextWriter writer);
    }
}