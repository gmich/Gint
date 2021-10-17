using System;
using System.Threading.Tasks;

namespace Gint
{
    internal class EvaluationMiddlewareFromDelegate : IEvaluationMiddleware
    {
        private readonly Func<CommandExecutionContext, Func<Task<CommandResult>>, Task<CommandResult>> middlewareDelegate;

        public EvaluationMiddlewareFromDelegate(Func<CommandExecutionContext, Func<Task<CommandResult>>, Task<CommandResult>> middlewareDelegate)
        {
            this.middlewareDelegate = middlewareDelegate;
        }

        public Task<CommandResult> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<CommandResult>> next)
        {
            return middlewareDelegate(commandExecutionContext, next);
        }
    }
}
