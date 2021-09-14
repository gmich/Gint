using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Gint
{
    internal class HelpCommandDefinition : ICommandDefinition
    {
        private CommandRegistry registry;

        public void Register(CommandRegistry registry)
        {
            this.registry = registry;

            registry.Add(
                  new CommandWithVariable("help", required: false, HelpHelp, Help),
                  new Option(1, "-d", "--detail", false, Detail, DetailHelp)
              );
        }

        private Task<ICommandOutput> Help(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            var printDetailed = input.Options.Contains("-d");

            // if there's a variable, show help for that command only
            if (!string.IsNullOrEmpty(input.Variable))
            {
                if (registry.Registry.ContainsKey(input.Variable))
                {
                    var entry = registry.Registry[input.Variable];
                    PrintCommand(ctx, entry);
                    if (printDetailed)
                        PrintCommandOptions(ctx, entry, printDetailed);
                }
                else
                {
                    ctx.Error.WriteLine($"Command <{input.Variable}> does not exist in the registry.");
                    return CommandOutput.ErrorTask;
                }
            }
            else
            {
                var count = 0;
                foreach (var cmd in registry.Registry)
                {
                    PrintCommand(ctx, cmd.Value);
                    if (printDetailed)
                        PrintCommandOptions(ctx, cmd.Value, printDetailed);

                    count++;
                    if (count < registry.Registry.Count)
                        ctx.OutStream.WriteLine();
                }
            }
            return CommandOutput.SuccessfulTask;
        }

        private static void PrintCommand(CommandExecutionContext ctx, CommandEntry cmd)
        {
            var entry = cmd.Command;
            ctx.OutStream.Write(entry.CommandName).WriteWhitespace();
            if (entry is CommandWithVariable cwv)
            {
                ctx.OutStream.Format(FormatType.DarkGrayForeground)
                    .Write("<var")
                    .Write(cwv.Required ? string.Empty : "?")
                    .Write(">")
                    .ClearFormat()
                    .WriteWhitespace();
            }
            PrintCommandOptions(ctx, cmd, false);
            ctx.OutStream.WriteLine()
                .Intent()
                .Format(FormatType.YellowForeground);
            entry.HelpCallback(ctx.OutStream);
            ctx.OutStream.WriteLine().ClearFormat();
        }

        private void HelpHelp(Out @out)
        {
            @out.Write("Help on command usage and examples.");
        }

        private Task<ICommandOutput> Detail(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            return CommandOutput.SuccessfulTask;
        }

        private void DetailHelp(Out @out)
        {
            @out.Write("Includes options details.");
        }


        private static void PrintCommandOptions(CommandExecutionContext ctx, CommandEntry entry, bool detailed)
        {
            foreach (var option in entry.Options)
            {
                string needsArgument = null;
                if (option is VariableOption vop)
                    needsArgument = " <var>";

                ctx.OutStream.Write($"[{option.Argument} / {option.LongArgument}");

                if (needsArgument != null)
                    ctx.OutStream.WriteFormatted(needsArgument, FormatType.DarkGrayForeground);

                ctx.OutStream.Write("]");

                if (detailed)
                    ctx.OutStream.WriteLine();
                else
                    ctx.OutStream.WriteWhitespace();


                if (detailed)
                {
                    ctx.OutStream.Intent().Format(FormatType.YellowForeground);
                    option.HelpCallback(ctx.OutStream);
                    ctx.OutStream.ClearFormat().WriteLine();
                }
            }
        }



    }
}
