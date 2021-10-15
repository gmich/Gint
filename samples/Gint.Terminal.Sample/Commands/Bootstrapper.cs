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
                    (input, ctx, next) =>
                    {
                        if (input.Options.Contains("-c"))
                        {
                            input.Scope.WriteString($"CPU count: {Environment.ProcessorCount}");
                        }
                        else
                            input.Scope.WriteString("CPU utilities");
                        return CommandOutput.SuccessfulTask;
                    })
                .AddOption(1, "-c", "--count", false, o => o.Write("Machine cpu count"),
                    (input, ctx, next) =>
                    {
                        return CommandOutput.SuccessfulTask;
                    });


            runtime.CommandRegistry.DiscoverAttributeDefinitions(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
