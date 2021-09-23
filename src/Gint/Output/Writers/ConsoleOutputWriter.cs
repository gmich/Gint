using System;
using System.Text;

namespace Gint
{
    public class ConsoleOutputWriter : OutputWriter
    {
        protected override void Format(OutputSyntaxToken token)
        {
            switch (FormatRegistry.GetFormatType(token.Value))
            {
                case FormatType.ResetFormat:
                    Console.ResetColor();
                    break;

                //Foreground
                case FormatType.RedForeground:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case FormatType.BlueForeground:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case FormatType.GreenForeground:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case FormatType.MagentaForeground:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case FormatType.DarkGrayForeground:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case FormatType.CyanForeground:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case FormatType.YellowForeground:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case FormatType.DarkYellowForeground:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;           
                case FormatType.BlackForeground:
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case FormatType.DarkBlueForeground:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case FormatType.DarkCyanForeground:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case FormatType.DarkGreenForeground:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case FormatType.DarkMagentaForeground:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case FormatType.DarkRedForeground:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case FormatType.GrayForeground:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case FormatType.WhiteForeground:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                //Background
                case FormatType.BlueBackground:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case FormatType.RedBackground:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case FormatType.BlackBackground:
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case FormatType.CyanBackground:
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case FormatType.DarkBlueBackground:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case FormatType.DarkCyanBackground:
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case FormatType.DarkGrayBackground:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case FormatType.DarkGreenBackground:
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case FormatType.DarkMagentaBackground:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case FormatType.DarkRedBackground:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case FormatType.DarkYellowBackground:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case FormatType.GrayBackground:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case FormatType.GreenBackground:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case FormatType.MagentaBackground:
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case FormatType.WhiteBackground:
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case FormatType.YellowBackground:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;

                case FormatType.Custom:
                    break;
            }
        }

        protected override void NewLine(OutputSyntaxToken token)
        {
            Console.WriteLine();
        }

        protected override void PrintText(OutputSyntaxToken token)
        {
            Console.Write(token.Value);
        }

        protected override void EndOfStream(OutputSyntaxToken token)
        {
        }

        protected override void StartOfStream()
        {
        }

        public override void Flush()
        {
        }
    }

}
