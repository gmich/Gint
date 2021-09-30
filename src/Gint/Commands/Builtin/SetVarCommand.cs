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

        public Task<ICommandOutput> SetVar(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (!ctx.GlobalScope.ContainsKey(input.Variable))
            {
                ctx.GlobalScope.Add(input.Variable, input.Scope.ReadInputAsString());
                return CommandOutput.SuccessfulTask;

            }
            else
            {
                ctx.Error.Write($"Variable {input.Variable} is already set.");
                return CommandOutput.ErrorTask;
            }
        }

        public static void Help(Out o)
        {
            o.Write("Sets variable.");
        }


    }

}
