﻿using Gint.Markup;
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
                    (ctx, next) =>
                    {
                        if (ctx.ExecutingCommand.Options.Contains("-c"))
                        {
                            ctx.Scope.WriteString($"CPU count: {Environment.ProcessorCount}{Environment.NewLine}");
                        }
                        else
                            ctx.Scope.WriteString($"CPU utilities{Environment.NewLine}");
                        return CommandOutput.SuccessfulTask;
                    })
                .AddOption(1, "-c", "--count", false, o => o.Write("Machine cpu count"),
                    (ctx, next) =>
                    {
                        return CommandOutput.SuccessfulTask;
                    });


            runtime.CommandRegistry.DiscoverAttributeDefinitions(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}