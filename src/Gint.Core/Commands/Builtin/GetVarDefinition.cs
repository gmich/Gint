using System;
using System.Threading.Tasks;

namespace Gint.Builtin
{
    internal sealed class GetVarDefinition : ICommandDefinition
    {
        public void Register(CommandRegistry registry)
        {
            registry.AddVariableCommand("getvar", required: true, Help, GetVar);
        }

        public Task<CommandResult> GetVar(CommandExecutionContext ctx)
        {
            if (ctx.GlobalScope.ContainsKey(ctx.ExecutingCommand.Variable))
            {
                ctx.Scope.WriteString((string)ctx.GlobalScope[ctx.ExecutingCommand.Variable]);
                return CommandResult.SuccessfulTask;
            }
            else
            {
                ctx.Error.Write($"Variable {ctx.ExecutingCommand.Variable} is not set.");
                return CommandResult.ErrorTask;
            }
        }

        public void Help(Out o)
        {
            o.Write("Retrieves variable.");
        }
    }
}
