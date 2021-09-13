using System;
using System.Threading.Tasks;

namespace Gint
{
    public interface IExecutionBlock
    {
        public ExecutionBlock Callback { get; }
        public HelpCallback HelpCallback { get; }
    }

    public delegate Task<ICommandOutput> ExecutionBlock(ICommandInput input, CommandExecutionContext executionContext, Func<Task> next);

    public delegate void HelpCallback(Out @out);


    public static class ExecutionBlockUtilities
    {
        public static Task<ICommandOutput> NoopExecutionBlock(ICommandInput input, CommandExecutionContext executionContext, Func<Task> next) => CommandOutput.SuccessfulTask;

        public static void NoopHelp(Out @out) {}
    }
}
