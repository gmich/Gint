using System;

namespace Gint.Markup.Sample
{
    class Program
    {
        static void Demo()
        {
            new TablePoc();
            Console.ReadLine();
            return;
            Console.WriteLine("Markup Demo");
            var document = new MarkupDocument();
            var fgred = document.AddFormatWithVariable("fg", "red");
            document.Write("Hello markup");
            fgred.Close();

            document.WriteLine().Timestamp().WriteWhitespace();

            var fgred_bgwhite = document.AddFormatWithVariable(("fg", "red"), ("bg", "white"));
            document.Write("Lorem Ipsum");
            fgred_bgwhite.Close();
            document.WriteLine();

            var txt = document.Buffer;
            Console.WriteLine(txt);
            Console.WriteLine();
            new ConsoleMarkupWriter().Print(txt);
            Console.WriteLine("-------------------------");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Demo();
            var writer = new ConsoleMarkupWriter();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("markup » ");
                Console.ResetColor();
                var markup = Console.ReadLine();
                Console.WriteLine();
                writer.Print(markup);
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
