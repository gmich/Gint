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
                  new Option(1, "-d", "--detail", false, Detail, DetailHelp),
                  new VariableOption(2, "-c", "--command", true, Command, CommandHelp)
              );
        }

        private Task<ICommandOutput> Help(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (!input.Options.Any())
            {
                foreach (var cmd in registry.Registry)
                {
                    PrintCommand(ctx, cmd.Value);
                }
            }

            return CommandOutput.SuccessfulTask;
        }

        private static void PrintCommand(CommandExecutionContext ctx, CommandEntry cmd)
        {
            var entry = cmd;
            ctx.OutStream.Write(entry.Command.CommandName).WriteWhitespace();
            foreach (var option in entry.Options)
            {
                ctx.OutStream.Write($"[{option.Argument} / {option.LongArgument}]").WriteWhitespace();
            }
            ctx.OutStream.WriteLine()
                .Intent()
                .Format(FormatType.ForegroundYellow);
            entry.Command.HelpCallback(ctx.OutStream);
            ctx.OutStream.ClearFormat();
        }

        private void HelpHelp(Out @out)
        {
            @out.WriteLine("Help on command usage and examples.");
        }

        private Task<ICommandOutput> Detail(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            ctx.Info.WriteLine($"not implemented yet :p");
            return CommandOutput.SuccessfulTask;
        }

        private void DetailHelp(Out @out)
        {
            @out.Write("Details.");
        }

        private Task<ICommandOutput> Command(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (registry.Registry.ContainsKey(input.Variable))
            {
                var entry = registry.Registry[input.Variable];

                PrintCommand(ctx, entry);

                ctx.OutStream.WriteLine();
                foreach (var option in entry.Options)
                {
                    ctx.OutStream.WriteLine($"[{option.Argument} / {option.LongArgument}]");

                    ctx.OutStream.Intent().Format(FormatType.ForegroundYellow);
                    option.HelpCallback(ctx.OutStream);
                    ctx.OutStream.ClearFormat().WriteLine();
                }
            }
            else
            {
                ctx.Error.WriteLine($"Command <{input.Variable}> does not exist in the registry.");
                return CommandOutput.ErrorTask;
            }

            return CommandOutput.SuccessfulTask;
        }

        private void CommandHelp(Out @out)
        {
            @out.Write("Provides details for the specified command.");
        }


    }
}
