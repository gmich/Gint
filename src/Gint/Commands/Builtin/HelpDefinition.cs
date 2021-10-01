using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Gint.Markup;

namespace Gint.Builtin
{
    internal class HelpDefinition : ICommandDefinition
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
            var writer = new Out();
            var printDetailed = input.Options.Contains("-d");

            // if there's a variable, show help for that command only
            if (!string.IsNullOrEmpty(input.Variable))
            {
                if (registry.Collection.ContainsKey(input.Variable))
                {
                    var entry = registry.Collection[input.Variable];
                    PrintCommand(writer,ctx, entry);
                    if (printDetailed)
                        PrintCommandOptions(writer,ctx, entry, printDetailed);
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
                foreach (var cmd in registry.Collection)
                {
                    PrintCommand(writer,ctx, cmd.Value);
                    if (printDetailed)
                        PrintCommandOptions(writer,ctx, cmd.Value, printDetailed);

                    count++;
                    if (count < registry.Collection.Count)
                        writer.WriteLine();
                }
            }
            input.Scope.WriteString(writer.Buffer);
            return CommandOutput.SuccessfulTask;
        }

        private static void PrintCommand(Out writer,CommandExecutionContext ctx, CommandEntry cmd)
        {
            var entry = cmd.Command;
            writer.Write(entry.CommandName).WriteWhitespace();
            if (entry is CommandWithVariable cwv)
            {
                var required = cwv.Required ? string.Empty : "?";
                writer.WithForegroundColor().DarkGray()
                    .Write($"<var{required}>")
                    .WriteWhitespace();
            }
            PrintCommandOptions(writer, ctx, cmd, false);
            var format = writer.WriteLine()
                .Intent()
                .WithForegroundColor()
                .Yellow();
            entry.HelpCallback(writer);
            writer.WriteLine();

            format.End();
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


        private static void PrintCommandOptions(Out writer, CommandExecutionContext ctx, CommandEntry entry, bool detailed)
        {
            foreach (var option in entry.Options)
            {
                string needsArgument = null;
                if (option is VariableOption vop)
                    needsArgument = " <var>";

                writer.Write($"[{option.Argument} / {option.LongArgument}");

                if (needsArgument != null)
                    writer.WithForegroundColor().DarkGray().Write(needsArgument);

                writer.Write("]");

                if (detailed)
                    writer.WriteLine();
                else
                    writer.WriteWhitespace();


                if (detailed)
                {
                    var format = writer.Intent().WithForegroundColor().Yellow();
                    option.HelpCallback(writer);
                    format.End().WriteLine();
                }
            }            
        }



    }
}
