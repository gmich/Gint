﻿using Gint.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Commands.Sample
{
    public static class Bootstrapper
    {
        public static void RegisterCommands(CommandRuntime runtime)
        {
            runtime.CommandRegistry
                .AddCommand("cpu", o => o.Write("CPU utilities"),
                    (ctx, next) =>
                    {
                        if (ctx.ExecutingCommand.Options.Contains("-c"))
                        {
                            ctx.Scope.WriteString($"CPU count: {Environment.ProcessorCount}");
                        }
                        else
                            ctx.Scope.WriteString("CPU utilities");
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
