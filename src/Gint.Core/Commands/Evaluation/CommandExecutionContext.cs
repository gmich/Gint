using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{
    public class CommandExecutionContext
    {
        internal CommandExecutionContext(ExecutingCommand cmd, CommandScope scope, GlobalExecutionContext ctx)
        {
            ExecutingCommand = cmd;
            Scope = scope;
            Info = ctx.Info;
            Error = ctx.Error;
            CancellationToken = ctx.CancellationToken;
            GlobalScope = ctx.GlobalScope;
        }

        public ExecutingCommand ExecutingCommand { get; }
        public CommandScope Scope { get; }
        public Out Info { get; }
        public Out Error { get; }

        public Dictionary<string, object> GlobalScope { get; }
        public CancellationTokenSource CancellationToken { get; }
    }

}
