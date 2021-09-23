using System;

namespace Gint
{
    public class CommandDiscoveryException : Exception
    {
        public CommandDiscoveryException(string message) : base(message)
        {
        }

        public CommandDiscoveryException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
