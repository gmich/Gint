namespace Gint
{
    public enum FormatType
    {
        [FormatMask("000")] ResetFormat,

        [FormatMask("B01")] BackgroundRed,

        [FormatMask("B02")] BackgroundBlue,

        [FormatMask("F01")] ForegroundRed,

        [FormatMask("F02")] ForegroundBlue,

        [FormatMask("F03")] ForegroundGreen,

        [FormatMask("F04")] ForegroundMagenta,

        [FormatMask("F05")] ForegroundDarkGray,

        [FormatMask("F06")] ForegroundCyan,

        [FormatMask("F07")] ForegroundYellow,

        [FormatMask("F08")] ForegroundDarkYellow,

        Custom
    }


}
