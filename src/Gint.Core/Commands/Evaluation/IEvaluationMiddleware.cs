using System;
using System.Threading.Tasks;

namespace Gint
{
    public interface IEvaluationMiddleware
    {
        Task<CommandResult> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<CommandResult>> next);
    }
}
