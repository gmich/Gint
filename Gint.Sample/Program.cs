using Gint.Markup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal class Program
    {
        static void Markup()
        {
            var text = @"
[fg.red,bg:white]hello world[-]!!!!!![-fg.red]

[~br,~br]

[fg.green]hello world[-fg.green]!!!!!!

[~br,~br]

[fg.green]hello world[-]!!!!!!

[~br,~br]

[test:variable with space]hello world[-]!!!!!!

[~br,~br]

[[[~date:HH:mm tt]]] hello world!!!!!!

[~br,~br]

[~date:HH:mm,fg.red] hello world[-]!!!!!!
";
            //Console.WriteLine(text);
            //new Markup.ConsoleMarkupWriter().Print(text);

            var document = new MarkupDocument();
            document.Write("test");
            document.NewLine();
            document.WriteWithinRedFontWithWhiteBackground("Hello world!!!");
            document.NewLine();
            document.WriteDate();
            document.NewLine();
            var fgred= document.StartFormat("fg.red");
            document.Write("red font");
            fgred.Close();

            var txt = document.Document;
            Console.WriteLine(txt);
            new Markup.ConsoleMarkupWriter().Print(txt);
        }

        static async Task Main()
        {
            Markup();

            var consoleOutputWriter = new ConsoleOutputWriter();

            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                commandExecutionContextFactory: () =>
                {
                    var ctx = CommandExecutionContext.New;
                    ctx.Info.AddWriter(consoleOutputWriter);
                    ctx.Error.AddWriter(consoleOutputWriter);
                    ctx.GlobalScope.Add("di_container", "you can add a di container for example, or resolve a dependency");
                    return ctx;
                });

            Bootstrapper.RegisterCommands(runtime);

            //cancel command on control + c
            runtime.OnCommandExecuting += (sender, args) =>
            {
                Console.CancelKeyPress += (s, a) =>
                {
                    args.CommandExecutionContext.CancellationToken.Cancel();
                };
            };

            //log interpretater results details
            //runtime.Options.LogBindTree = true;
            //runtime.Options.LogParseTree = true;
            runtime.Options.Out.AddWriter(consoleOutputWriter);

            while (true)
            {
                //print prompt
                runtime.Options.Out
                    .Format(FormatType.GreenForeground)
                    .Write($"{Environment.MachineName} » ")
                    .Flush();

                await runtime.Run(Console.ReadLine());
            }
        }
    }
}
