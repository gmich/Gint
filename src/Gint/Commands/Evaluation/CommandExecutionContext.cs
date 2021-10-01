using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{

    public class CommandExecutionContext
    {
        public CommandExecutionContext(Out info, Out error)
        {
            Info = info;
            Error = error;
            CancellationToken = new CancellationTokenSource();
            GlobalScope = new Dictionary<string, object>();
        }

        public Out Info { get; }
        public Out Error { get; }
        public Dictionary<string, object> GlobalScope { get; }

        public CancellationTokenSource CancellationToken { get; }

        public static CommandExecutionContext Empty => new(new Out(), new Out());
    
    }

}
