using System;
using System.Threading.Tasks;
using System.Linq;

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
            if (!input.Options.Any())
            {
                var count = 0;
                foreach (var cmd in registry.Registry)
                {
                    PrintCommand(ctx, cmd.Value);
                    count++;
                    if (count < registry.Registry.Count)
                        ctx.OutStream.WriteLine();
                }
            }

            return CommandOutput.SuccessfulTask;
        }

        private static void PrintCommand(CommandExecutionContext ctx, CommandEntry cmd)
        {
            var entry = cmd;
            ctx.OutStream.Write(entry.Command.CommandName).WriteWhitespace();
            PrintCommandOptions(ctx, cmd, false);
            ctx.OutStream.WriteLine()
                .Intent()
                .Format(FormatType.ForegroundYellow);
            entry.Command.HelpCallback(ctx.OutStream);
            ctx.OutStream.WriteLine().ClearFormat();
        }

        private void HelpHelp(Out @out)
        {
            @out.Write("Help on command usage and examples.");
        }

        private Task<ICommandOutput> Detail(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            var count = 0;
            foreach (var cmd in registry.Registry)
            {
                PrintCommand(ctx, cmd.Value);
                PrintCommandOptions(ctx, cmd.Value, true);
                count++;
                if (count < registry.Registry.Count)
                    ctx.OutStream.WriteLine();
            }

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
                var entry = registry.Registry[input.Variable];

                PrintCommand(ctx, entry);
                PrintCommandOptions(ctx, entry, true);
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
                    ctx.OutStream.WriteFormatted(needsArgument, FormatType.ForegroundDarkGray);

                ctx.OutStream.Write("]");

                if (detailed)
                    ctx.OutStream.WriteLine();
                else
                    ctx.OutStream.WriteWhitespace();


                if (detailed)
                {
                    ctx.OutStream.Intent().Format(FormatType.ForegroundYellow);
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
