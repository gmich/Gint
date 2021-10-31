using System;

namespace Gint.Markup.Sample
{
    public class FluentTablePoc
    {
        public FluentTablePoc()
        {

            var renderer = GintTable
                .WithFirstRowAsHeader()
                    .AddColumn("header1")
                    .AddColumn("header2")
                    .AddColumn("header3")
                 .NewRow()
                    .RenderEmptyRow()
                 .WithRowAlignment(Alignment.Start)
                    .AddColumn("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum")
                    .AddColumn("green content")
                        .WithColor(ConsoleColor.Green)
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
                            OnOverflow = TextOverflowOption.ChangeLine
                        },
                        TableStyle = SquareTable.Style,
                        TryFitToScreen = true,
                        ColumnPaddingLeft = 2,
                        ColumnPaddingRight = 2,
                    }
                    .WithColorPallette(border: ConsoleColor.DarkYellow, header: ConsoleColor.DarkGray, content: ConsoleColor.DarkMagenta)
                    .WithHeaderUppercase()
                );

            renderer.Render(Console.Out);
        }
    }

}
