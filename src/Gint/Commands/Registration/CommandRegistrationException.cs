using System;

namespace Gint
{
    public class CommandRegistrationException : Exception
    {
        public CommandRegistrationException(string message) : base(message)
        {
        }

        public CommandRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
