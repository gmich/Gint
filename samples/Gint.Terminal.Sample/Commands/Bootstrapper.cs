using Gint.Markup;
using System;
using System.Linq;

namespace Gint.Terminal.Sample
{
    public static class Bootstrapper
    {
        public static void RegisterCommands(this CommandRuntime runtime)
        {
            runtime.CommandRegistry
                .AddCommand("cpu", o => o.Write("CPU utilities"),
                    (ctx) =>
                    {
                        if (ctx.ExecutingCommand.Options.Contains("-c"))
                        {
                            ctx.Scope.WriteString($"CPU count: {Environment.ProcessorCount}{Environment.NewLine}");
                        }
                        else
                            ctx.Scope.WriteString($"CPU utilities{Environment.NewLine}");
                        return CommandResult.SuccessfulTask;
                    })
                .AddOption("-c", "--count", o => o.Write("Machine cpu count"),
                    (ctx) =>
                    {
                        return CommandResult.SuccessfulTask;
                    });


            runtime.CommandRegistry.AddDefinition(new ExampleCommand());
            runtime.CommandRegistry.DiscoverAttributeDefinitions(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
