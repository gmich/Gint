using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint
{


    public delegate Task<ICommandOutput> ExecutionBlock(ICommandInput input, CommandExecutionContext executionContext, Func<Task> next);

    public delegate void HelpCallback(Out @out);

    public delegate IEnumerable<Suggestion> SuggestionsCallback(string variable);


    public static class CallbackUtilities
    {
        public static Task<ICommandOutput> NoopExecutionBlock(ICommandInput input, CommandExecutionContext executionContext, Func<Task> next) => CommandOutput.SuccessfulTask;

        public static void NoopHelp(Out @out) {}

        public static IEnumerable<Suggestion> EmptySuggestions(string variable) => Enumerable.Empty<Suggestion>();
    }
}
