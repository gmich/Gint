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
[fg:red,bg:white]hello world[-bg:white]!!!!!![-fg:red]

[~br,~br]

[fg:green]hello world[-fg:green]!!!!!!

";
            Console.WriteLine(text);

            new Markup.ConsoleMarkupWriter().Print(text);
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
