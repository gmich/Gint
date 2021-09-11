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
                case FormatType.BackgroundRed:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case FormatType.ForegroundRed:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case FormatType.ForegroundBlue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case FormatType.BackgroundBlue:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case FormatType.ForegroundGreen:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case FormatType.ForegroundMagenta:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case FormatType.ForegroundDarkGray:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case FormatType.ForegroundCyan:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case FormatType.ForegroundYellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case FormatType.ForegroundDarkYellow:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
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
