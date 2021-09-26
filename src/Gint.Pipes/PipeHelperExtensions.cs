using System.Text;

namespace Gint.Pipes
{
    public static class PipeHelperExtensions
    {
        public static string AsUTF8String(this byte[] bytes)
        {
            var utf8 = Encoding.UTF8 as UTF8Encoding;
            return utf8.GetString(bytes);
        }

        public static byte[] AsUTF8ByteArray(this string utf8String)
        {
            var utf8 = Encoding.UTF8 as UTF8Encoding;
            return utf8.GetBytes(utf8String);
        }
    }
}
