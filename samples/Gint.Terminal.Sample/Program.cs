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

            AddMiddlewares(runtime);

            var terminal = new Terminal(new TerminalOptions
            {
                CommandHistoryRepository = new PokemonFileHistoryRepository(),
                Registry = runtime.CommandRegistry,
                MaxSuggestionsPerRow = 5
            })
            .BindRuntime(runtime);

            while (true)
            {
                terminal.WaitForInput();
            }
        }

        private static void AddMiddlewares(CommandRuntime runtime)
        {
            runtime.Options.WithMiddleware(async (ctx, next) =>
            {
                ctx.Info
                .WithForegroundColor()
                .DarkYellow()
                .WriteLine($"[{ctx.ExecutingCommand.ExecutionId}]Executing: {ctx.ExecutingCommand.ExecutionSpan}")
                .Flush();

                var res = await next();

                var input = ctx.Scope.ReadInputAsString(advanceCursor: true);
                ctx.Info.WriteRaw(input);

                ctx.Info
                .WithForegroundColor()
                .DarkYellow()
                .WriteLine($"[{ctx.ExecutingCommand.ExecutionId}]Executed: {ctx.ExecutingCommand.ExecutionSpan}")
                .Flush();

                return res;
            });

        }
    }
}
