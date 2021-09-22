namespace Gint.Commands.Streaming
{
    public class UTF8Downstream : IUTF8Downstream
    {
        private readonly string stream;

        public UTF8Downstream(string stream)
        {
            this.stream = stream;
        }

        public string Drain()
        {
            return stream;
        }
    }
}
