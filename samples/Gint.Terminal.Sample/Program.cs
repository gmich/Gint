using System;

namespace Gint.Terminal.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                options: CommandRuntimeOptions.DefaultConsole
            );

            runtime.RegisterCommands();
            //AddMiddlewares(runtime);

            var terminal = new CommandTerminal(runtime, new TerminalOptions
            {
                CommandHistoryRepository = new PokemonFileHistoryRepository(),
                MaxSuggestionsPerRow = 5,
            });

            while (true)
            {
                terminal.WaitForInput();
            }
        }

        private static void AddMiddlewares(CommandRuntime runtime)
        {
            runtime.Options.AddMiddleware(async (ctx, next) =>
            {
                ctx.Info
                .WithForegroundColor().DarkYellow()
                .WriteLine($"[{ctx.ExecutingCommand.ExecutionId}]Executing: {ctx.ExecutingCommand.ExecutionSpan}")
                .Flush();

                var res = await next();

                var input = ctx.Scope.ReadInputAsString(advanceCursor: true);
                ctx.Info.WriteRaw(input);

                ctx.Info
                .WithForegroundColor().DarkYellow()
                .WriteLine($"[{ctx.ExecutingCommand.ExecutionId}]Executed: {ctx.ExecutingCommand.ExecutionSpan}")
                .Flush();

                return res;
            });

        }
    }
}
