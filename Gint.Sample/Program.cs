using System;
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
                    var ctx = CommandExecutionContext.Default;
                    ctx.Info.AddWriter(consoleOutputWriter);
                    ctx.Error.AddWriter(consoleOutputWriter);
                    ctx.Metadata.Add("di_container", "you can add a di container for example, or resolve a dependency");
                    return ctx;
                });

            runtime.CommandRegistry
                .AddCommand("cpu", o => o.Write("Cpu utilities"),
                    (input, ctx, next) =>
                    {
                        ctx.OutStream.Write("CPU utilities");
                        return CommandOutput.SuccessfulTask;
                    })
                .AddOption(1, "-c", "--count", false, o => o.Write("Machine cpu count"),
                    (input, ctx, next) =>
                    {
                        ctx.OutStream.Write("CPU count: ")
                        .WriteFormatted($"{Environment.ProcessorCount}", FormatType.DarkGrayForeground);
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
            //runtime.Options.LogBindTree = true;
            //runtime.Options.LogParseTree = true;
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
