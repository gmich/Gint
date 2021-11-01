using System.IO;

namespace Gint.Tables
{
    public interface ITableRenderer
    {
        void Render(TextWriter writer);
    }
}