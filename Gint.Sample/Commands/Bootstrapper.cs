using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Sample
{
    public static class Bootstrapper
    {
        public static void RegisterCommands(CommandRuntime runtime)
        {
            runtime.CommandRegistry
                .AddCommand("cpu", o => o.Write("CPU utilities"),
                    (input, ctx, next) =>
                    {
                        if (input.Options.Contains("-c"))
                        {
                            ctx.OutStream.Write("CPU count: ")
                            .WriteFormatted($"{Environment.ProcessorCount}", FormatType.DarkGrayForeground);
                        }
                        else
                            ctx.OutStream.Write("CPU utilities");
                        return CommandOutput.SuccessfulTask;
                    })
                .AddOption(1, "-c", "--count", false, o => o.Write("Machine cpu count"),
                    (input, ctx, next) =>
                    {
                        return CommandOutput.SuccessfulTask;
                    });


            runtime.CommandRegistry
                .AddCommand("showlog", o => o.Write("Toggles showing of bind and parse trees."),
                    (input, ctx, next) =>
                    {
                        runtime.Options.LogBindTree = !runtime.Options.LogBindTree;
                        runtime.Options.LogParseTree = !runtime.Options.LogParseTree;

                        ctx.Info.Write("Showing bind / parse trees: ");
                        if (runtime.Options.LogBindTree)
                            ctx.Info.WriteFormatted("✓", FormatType.GreenForeground);
                        else
                            ctx.Info.WriteFormatted("x", FormatType.RedForeground);

                        return CommandOutput.SuccessfulTask;
                    });


            runtime.CommandRegistry.DiscoverAttributeDefinitions(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
