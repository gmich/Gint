using Gint.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Terminal.Sample
{
    internal sealed class ExampleCommand : ICommandDefinition
    {
        public void Register(CommandRegistry registry)
        {
            registry.Add(
                  new Command("example", helpCallback: Help, callback: Example),
                  new Option(1, "-t", "--table", allowMultiple: false, callback: CallbackUtilities.NoopExecutionBlock, helpCallback: TableHelp)
              );
        }

        public Task<CommandResult> Example(CommandExecutionContext ctx)
        {
            if (ctx.ExecutingCommand.Options.Contains("-t"))
            {
                ctx.Info.Write("Printing an example table...")
                    .WriteLine()
                    .WriteTable(GetExampleTable());
            }

            return CommandResult.SuccessfulTask;
        }


        public static void Help(Out o)
        {
            o.Write("Gint examples");
        }

        private Task<CommandResult> Detail(CommandExecutionContext ctx)
        {
            return CommandResult.SuccessfulTask;
        }

        private void TableHelp(Out @out)
        {
            @out.Write("Prints an example table.");
        }

        private static TableDefinition GetExampleTable()
        {
            var preferences =
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
                .WithColorPallette(border: ConsoleColor.DarkYellow, header: ConsoleColor.DarkGray, content: ConsoleColor.DarkMagenta);

            var table = GintTable
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
                    .AddColumn("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standa")
                        .WithForegroundColor(ConsoleColor.Green)
                    .AddColumn($"centered content")
                        .WithAlignment(Alignment.Center)
                .NewRow()
                .WithoutRowDivider()
                .WithRowAlignment(Alignment.Center)
                    .AddColumn($"hello world!!")
                 .Build(preferences);

            return table;
        }

    }

}
