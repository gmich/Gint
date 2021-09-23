namespace Gint
{
    public enum FormatType
    {
        [FormatMask("000")] ResetFormat,

        #region Background

        [FormatMask("B00")] BlackBackground,
                     
        [FormatMask("B01")] BlueBackground,
                     
        [FormatMask("B02")] CyanBackground,
                     
        [FormatMask("B03")] DarkBlueBackground,
                          
        [FormatMask("B04")] DarkCyanBackground,
                     
        [FormatMask("B05")] DarkGrayBackground,
                     
        [FormatMask("B06")] DarkGreenBackground,
                     
        [FormatMask("B07")] DarkMagentaBackground,
                     
        [FormatMask("B08")] DarkRedBackground,
                     
        [FormatMask("B09")] DarkYellowBackground,
                     
        [FormatMask("B0A")] GrayBackground,
                     
        [FormatMask("B0B")] GreenBackground,
                     
        [FormatMask("B0C")] MagentaBackground,
                     
        [FormatMask("B0D")] RedBackground,
                     
        [FormatMask("B0E")] WhiteBackground,
                     
        [FormatMask("B0F")] YellowBackground,

        #endregion

        #region Foreground

        [FormatMask("F00")] BlackForeground,
        
        [FormatMask("F01")] BlueForeground,

        [FormatMask("F02")] CyanForeground,

        [FormatMask("F03")] DarkBlueForeground,

        [FormatMask("F04")] DarkCyanForeground,

        [FormatMask("F05")] DarkGrayForeground,

        [FormatMask("F06")] DarkGreenForeground,

        [FormatMask("F07")] DarkMagentaForeground,

        [FormatMask("F08")] DarkRedForeground,

        [FormatMask("F09")] DarkYellowForeground,

        [FormatMask("F0A")] GrayForeground,

        [FormatMask("F0B")] GreenForeground,

        [FormatMask("F0C")] MagentaForeground,

        [FormatMask("F0D")] RedForeground,

        [FormatMask("F0E")] WhiteForeground,

        [FormatMask("F0F")] YellowForeground,

        #endregion

        Custom
    }


}
