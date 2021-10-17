using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint
{


    public delegate Task<CommandResult> ExecutionBlock(CommandExecutionContext executionContext);

    public delegate void HelpCallback(Out @out);

    public delegate IEnumerable<Suggestion> SuggestionsCallback(string variable);


    public static class CallbackUtilities
    {
        public static Task<CommandResult> NoopExecutionBlock(CommandExecutionContext executionContext) => CommandResult.SuccessfulTask;

        public static void NoopHelp(Out @out) {}

        public static IEnumerable<Suggestion> EmptySuggestions(string variable) => Enumerable.Empty<Suggestion>();
    }
}
