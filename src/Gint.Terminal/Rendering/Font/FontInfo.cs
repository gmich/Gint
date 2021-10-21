using System.Runtime.InteropServices;

namespace Gint.Terminal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontInfo
    {
        internal int cbSize;
        internal int FontIndex;
        internal short FontWidth;

        public short FontSize { get; internal set; }
        public int FontFamily { get; internal set; }
        public int FontWeight { get; internal set; }
        public string FontName => _fontName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
        public string _fontName;
    }
}
