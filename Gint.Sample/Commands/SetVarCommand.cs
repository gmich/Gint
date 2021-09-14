using System;
using System.Threading.Tasks;

namespace Gint.Sample
{
    internal sealed class SetVarCommand
    {
        [CommandWithVariable("setvar", required: true, helpCallback: nameof(Help))]
        public Task<ICommandOutput> SetVar(ICommandInput input, CommandExecutionContext ctx, Func<Task> next)
        {
            if (!ctx.GlobalScope.ContainsKey(input.Variable))
            {
                ctx.GlobalScope.Add(input.Variable, input.Stream.Raw);
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
