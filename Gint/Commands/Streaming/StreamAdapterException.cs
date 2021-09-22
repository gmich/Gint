using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Commands.Streaming
{

    public class StreamAdapterException : Exception
    {
        public StreamAdapterException(string message) : base(message)
        {
        }

        public StreamAdapterException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
