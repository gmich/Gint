using System;

namespace Gint.SyntaxHighlighting
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                options: CommandRuntimeOptions.DefaultConsole
              );

            var inputManager = new ConsoleInputManager(new ConsoleInputOptions
            {
                CommandHistoryRepository = new PokemonFileHistoryRepository(),
                Registry = runtime.CommandRegistry
            });

            inputManager.OnCommandReady += async (sender, cmd) =>
            {
                inputManager.AcceptInput = false;
                Console.WriteLine();
                await runtime.Run(cmd);
                inputManager.AcceptInput = true;
            };

            while (true)
            {
                inputManager.WaitNext();
            }
        }
    }
}
