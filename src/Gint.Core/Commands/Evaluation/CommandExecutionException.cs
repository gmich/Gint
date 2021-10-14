using System;

namespace Gint
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(string message) : base(message)
        {
        }

        public CommandExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
