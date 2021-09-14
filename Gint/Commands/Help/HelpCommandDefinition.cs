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
                  new Command("help", HelpHelp, Help),
                  new Option(1, "-d", "--detailed", false, Detail, DetailHelp),
                  new VariableOption(2, "-c", "--command", true, Command, CommandHelp)
              );
        }

        private Task<ICommandOutput> Help(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            var printDetailed = input.Options.Contains("-d");

            if (input.Options.Contains("-c"))
            {
                var cmds = (List<string>)input.Scope.Metadata["-c"];
                var count = 0;
                foreach (var cmd in cmds)
                {
                    var entry = registry.Registry[cmd];

                    PrintCommand(ctx, entry);
                    if (printDetailed)
                        PrintCommandOptions(ctx, entry, printDetailed);

                    count++;
                    if (count < cmds.Count)
                        ctx.OutStream.WriteLine();
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

        private Task<ICommandOutput> Command(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (registry.Registry.ContainsKey(input.Variable))
            {
                if (input.Scope.Metadata.ContainsKey("-c"))
                    ((List<string>)input.Scope.Metadata["-c"]).Add(input.Variable);
                else
                    input.Scope.Metadata.Add("-c", new List<string>() { input.Variable });

            }
            else
            {
                ctx.Error.WriteLine($"Command <{input.Variable}> does not exist in the registry.");
                return CommandOutput.ErrorTask;
            }

            return CommandOutput.SuccessfulTask;
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

        private void CommandHelp(Out @out)
        {
            @out.Write("Provides details for the specified command.");
        }


    }
}
