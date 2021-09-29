﻿using Gint.Markup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal class Program
    {

        static async Task Main()
        { 
            var consoleOutputWriter = new ConsoleMarkupWriter();

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
                    .WithForegroundColor().Green()
                    .Write($"{Environment.MachineName} » ");
                runtime.Options.Out.Flush();

                await runtime.Run(Console.ReadLine());
            }
        }
    }
}