using System;
using System.Threading.Tasks;

namespace Gint
{
    internal class EvaluationMiddlewareFromDelegate : IEvaluationMiddleware
    {
        private readonly Func<CommandExecutionContext, Func<Task<CommandOutput>>, Task<CommandOutput>> middlewareDelegate;

        public EvaluationMiddlewareFromDelegate(Func<CommandExecutionContext, Func<Task<CommandOutput>>, Task<CommandOutput>> middlewareDelegate)
        {
            this.middlewareDelegate = middlewareDelegate;
        }

        public Task<CommandOutput> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<CommandOutput>> next)
        {
            return middlewareDelegate(commandExecutionContext, next);
        }
    }
}
