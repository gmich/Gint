using System.Text;

namespace Gint.Commands.Streaming
{
    public class UTF8Upstream : IUTF8Upstream
    {
        public StringBuilder Builder { get; } = new StringBuilder();

        public void Push(string data)
        {
            Builder.Append(data);
        }
    }
}
