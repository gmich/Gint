using Gint.Markup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal class Program
    {

        static async Task Main()
        { 
            var runtime = new CommandRuntime(
                commandRegistry: CommandRegistry.Empty,
                options: CommandRuntimeOptions.DefaultConsole
              );

            Bootstrapper.RegisterCommands(runtime);

            while (true)
            {
                //print prompt
                runtime.Options.RuntimeInfo
                    .WithForegroundColor().Green()
                    .Write($"{Environment.MachineName} » ");
                runtime.Options.RuntimeInfo.Flush();

                await runtime.Run(Console.ReadLine());
            }
        }
    }
}
