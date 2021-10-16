using System;
using System.Threading.Tasks;

namespace Gint
{
    internal class EvaluationMiddlewareFromDelegate : IEvaluationMiddleware
    {
        private readonly Func<CommandExecutionContext, Func<Task<ICommandOutput>>, Task<ICommandOutput>> middlewareDelegate;

        public EvaluationMiddlewareFromDelegate(Func<CommandExecutionContext, Func<Task<ICommandOutput>>, Task<ICommandOutput>> middlewareDelegate)
        {
            this.middlewareDelegate = middlewareDelegate;
        }

        public Task<ICommandOutput> Intercept(CommandExecutionContext commandExecutionContext, Func<Task<ICommandOutput>> next)
        {
            return middlewareDelegate(commandExecutionContext, next);
        }
    }
}
