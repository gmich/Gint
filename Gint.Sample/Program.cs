using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Sample
{
    class Program
    {
        static async Task Main()
        {
            var consoleOutputWriter = new ConsoleOutputWriter();

            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                commandExecutionContextFactory: () =>
                {
                    var ctx = CommandExecutionContext.New;
                    ctx.Info.AddWriter(consoleOutputWriter);
                    ctx.Error.AddWriter(consoleOutputWriter);
                    ctx.GlobalMetadata.Add("di_container", "you can add a di container for example, or resolve a dependency");
                    return ctx;
                });

            runtime.CommandRegistry
                .AddVariableCommand("cpu", false, o => o.Write("CPU utilities"),
                    (input, ctx, next) =>
                    {
                        if(input.Options.Contains("-c"))
                        {
                            ctx.OutStream.Write("CPU count: ")
                            .WriteFormatted($"{Environment.ProcessorCount}", FormatType.DarkGrayForeground);
                        }
                        else
                            ctx.OutStream.Write("CPU utilities");
                        return CommandOutput.SuccessfulTask;
                    })
                .AddOption(1, "-c", "--count", false, o => o.Write("Machine cpu count"),
                    (input, ctx, next) =>
                    {
                        return CommandOutput.SuccessfulTask;
                    });

            //cancel command on control + c
            runtime.OnCommandExecuting += (sender, args) =>
            {
                Console.CancelKeyPress += (s, a) =>
                {
                    args.CommandExecutionContext.CancellationToken.Cancel();
                };
            };

            //log interpretater results details
            runtime.Options.LogBindTree = true;
            runtime.Options.LogParseTree = true;
            runtime.Options.Out.AddWriter(consoleOutputWriter);

            while (true)
            {
                //print prompt
                runtime.Options.Out
                    .Format(FormatType.GreenForeground)
                    .WriteLine($"{Environment.MachineName} cli »")
                    .WriteLine()
                    .Flush();

                await runtime.Run(Console.ReadLine());
            }
        }
    }
}
