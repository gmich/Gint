using System;
using System.Threading.Tasks;

namespace Gint.Builtin
{
    internal sealed class SetVarDefinition : ICommandDefinition
    {
        public void Register(CommandRegistry registry)
        {
            registry.AddVariableCommand("setvar", required: true, Help, SetVar);
        }

        public Task<CommandResult> SetVar(CommandExecutionContext ctx)
        {
            if (!ctx.GlobalScope.ContainsKey(ctx.ExecutingCommand.Variable))
            {
                ctx.GlobalScope.Add(ctx.ExecutingCommand.Variable, ctx.Scope.ReadInputAsString());
                return CommandResult.SuccessfulTask;

            }
            else
            {
                ctx.Error.Write($"Variable {ctx.ExecutingCommand.Variable} is already set.");
                return CommandResult.ErrorTask;
            }
        }

        public static void Help(Out o)
        {
            o.Write("Sets variable.");
        }


    }

}
