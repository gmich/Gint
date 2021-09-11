using Gint;
using Gint;
using System;
using System.Threading.Tasks;

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
                  new Option(1, "-d", "--detail", Detail, DetailHelp),
                  new VariableOption(2, "-c", "--command", Command, CommandHelp)
              );
        }

        private Task<ICommandOutput> Help(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            ctx.Info.WriteLine("help is not supported yet =/");

            return CommandOutput.SuccessfulTask;
        }

        private void HelpHelp(Out @out)
        {
            @out.Write("help");
        }

        private Task<ICommandOutput> Detail(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            ctx.Info.WriteLine("detail");
            return CommandOutput.SuccessfulTask;
        }

        private void DetailHelp(Out @out)
        {
            @out.Write("detail");
        }

        private Task<ICommandOutput> Command(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            ctx.Info.WriteLine($"command {input.Variable}");

            return CommandOutput.SuccessfulTask;
        }

        private void CommandHelp(Out @out)
        {
            @out.Write("command");
        }


    }
}
