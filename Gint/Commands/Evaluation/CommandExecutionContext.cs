using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gint
{

    public class CommandExecutionContext
    {
        public CommandExecutionContext(Out @out, Out info, Out error, CancellationTokenSource cancellationToken, Dictionary<string, object> globalMetadata, Dictionary<string, object> scopeMetadata)
        {
            OutStream = @out;
            Info = info;
            Error = error;
            CancellationToken = cancellationToken;
            GlobalMetadata = globalMetadata;
            ScopeMetadata = scopeMetadata;
        }

        public Out OutStream { get; }
        public Out Info { get; }
        public Out Error { get; }
        public Dictionary<string, object> ScopeMetadata { get; }
        public Dictionary<string, object> GlobalMetadata { get; }

        public CancellationTokenSource CancellationToken { get; }

        internal CommandExecutionContext Clone() => this;

        public static CommandExecutionContext New => new(new Out(), new Out(), new Out(), new CancellationTokenSource(), new Dictionary<string, object>(), new Dictionary<string, object>());
    
    }

}
