using System;

namespace Gint.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                options: CommandRuntimeOptions.DefaultConsole
            );

            var terminal = new Terminal(new TerminalOptions
            {
                CommandHistoryRepository = new PokemonFileHistoryRepository(),
                Registry = runtime.CommandRegistry
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
                terminal.WaitInput();
            }
        }
    }
}
