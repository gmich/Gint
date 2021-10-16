using System.Collections.Generic;
using System.Threading;

namespace Gint
{
    public class GlobalExecutionContext
    {
        public GlobalExecutionContext(Out info, Out error)
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


    }

}
