using System;
using System.Threading.Tasks;

namespace Gint
{
    public interface IEvaluationMiddleware
    {
        Task<ICommandOutput> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<ICommandOutput>> next);
    }
}
