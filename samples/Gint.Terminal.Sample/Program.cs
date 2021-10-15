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

            var terminal = new Terminal(new TerminalOptions
            {
                CommandHistoryRepository = new PokemonFileHistoryRepository(),
                Registry = runtime.CommandRegistry,
                MaxSuggestionsPerRow = 5
            });

            terminal.OnCommandReady += async (sender, cmd) =>
            {
                terminal.AcceptInput = false;
                Console.WriteLine();
                await runtime.Run(cmd);
                terminal.AcceptInput = true;
            };

            while (true)
            {
                terminal.WaitForInput();
            }
        }
    }
}
