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

        public Task<ICommandOutput> GetVar(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (ctx.GlobalScope.ContainsKey(input.Variable))
            {
                input.Scope.WriteString((string)ctx.GlobalScope[input.Variable]);
                return CommandOutput.SuccessfulTask;
            }
            else
            {
                ctx.Error.Write($"Variable {input.Variable} is not set.");
                return CommandOutput.ErrorTask;
            }
        }

        public void Help(Out o)
        {
            o.Write("Retrieves variable.");
        }
    }
}
