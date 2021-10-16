using System;
using System.Threading.Tasks;

namespace Gint
{
    public interface IEvaluationMiddleware
    {
        Task<CommandOutput> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<CommandOutput>> next);
    }
}
