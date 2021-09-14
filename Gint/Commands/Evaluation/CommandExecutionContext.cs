using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{

    public class CommandExecutionContext
    {
        public CommandExecutionContext(Out @out, Out info, Out error, CancellationTokenSource cancellationToken, Dictionary<string, object> globalMetadata)
        {
            OutStream = @out;
            Info = info;
            Error = error;
            CancellationToken = cancellationToken;
            GlobalScope = globalMetadata;
        }

        public Out OutStream { get; }
        public Out Info { get; }
        public Out Error { get; }
        public Dictionary<string, object> GlobalScope { get; }

        public CancellationTokenSource CancellationToken { get; }

        public static CommandExecutionContext New => new(new Out(), new Out(), new Out(), new CancellationTokenSource(), new Dictionary<string, object>());
    
    }

}
