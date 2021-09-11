using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{

    public class CommandExecutionContext
    {
        public CommandExecutionContext(Out @out, Out info, Out error, CancellationTokenSource cancellationToken)
        {
            OutStream = @out;
            Info = info;
            Error = error;
            CancellationToken = cancellationToken;
        }

        public Out OutStream { get; }
        public Out Info { get; }
        public Out Error { get; }
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
        public CancellationTokenSource CancellationToken { get; }


        public static CommandExecutionContext Default => new CommandExecutionContext(new Out(), new Out(), new Out(), new CancellationTokenSource());

    }

}
