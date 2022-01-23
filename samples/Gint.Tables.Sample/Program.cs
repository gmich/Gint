using System;

namespace Gint.Tables.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderTableWithDefaults();
            RenderSimpleTable();
            RenderComplexTable();

            Console.ReadLine();
        }

        private static void RenderTableWithDefaults()
        {
            GintTable
                .WithFirstRowAsHeader()
                    .AddColumn("header1")
                    .AddColumn("header2")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                 .Build()
                 .RenderToConsole();


            Console.WriteLine();
            Console.WriteLine();
        }

        private static void RenderSimpleTable()
        {
            GintTable
                .WithFirstRowAsHeader()
                    .AddColumn("header1")
                    .AddColumn("header2")
                    .AddColumn("header3")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                     .AddColumn("column3")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                     .AddColumn("column3")
                 .Build(
                    new TableRenderPreferences
                    {
                        TextOverflow = new TextOverflow
                        {
                            OnOverflow = TextOverflowOption.ChangeLine
                        },
                        TableStyle = TableStyle.OnlyHeaderUnderline,
                        TryFitToScreen = false,
                        ColumnPaddingLeft = 2,
                        ColumnPaddingRight = 2,
                        WithHeaderUpperCase = true,
                    }
                    .WithColorPallette(border: ConsoleColor.DarkYellow, header: ConsoleColor.DarkGray, content: ConsoleColor.DarkMagenta)
                )
                .RenderToConsole();


            Console.WriteLine();
            Console.WriteLine();
        }

        private static void RenderComplexTable()
        {
            GintTable
                .WithFirstRowAsHeader()
                    .AddColumn("header1")
                    .AddColumn("header2")
                    .AddColumn("header3")
                 .NewRow()
                     .AddColumn("column1")
                     .AddColumn("column2")
                 .NewRow()
                 .WithRowAlignment(Alignment.Start)
                    .AddColumn("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum")
                    .AddColumn("green content")
                        .WithForegroundColor(ConsoleColor.Green)
                    .AddColumn($"centered content")
                        .WithAlignment(Alignment.Center)
                .NewRow()
                .WithoutRowDivider()
                .WithRowAlignment(Alignment.Center)
                    .AddColumn($"hello world!!")
                .Build(
                    new TableRenderPreferences
                    {
                        TextOverflow = new TextOverflow
                        {
                            OnOverflow = TextOverflowOption.ChangeLine,
                        },
                        //PreferredTableWidth = Console.BufferWidth / 2,
                        TableStyle = TableStyle.Square,
                        TryFitToScreen = true,
                        ColumnPaddingLeft = 2,
                        ColumnPaddingRight = 2,
                        WithHeaderUpperCase = true
                    }
                    .WithColorPallette(border: ConsoleColor.DarkYellow, header: ConsoleColor.DarkGray, content: ConsoleColor.DarkMagenta)
                )
                .Render(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
