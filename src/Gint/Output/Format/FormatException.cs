using System;

namespace Gint
{
    public class FormatException : Exception
    {
        public FormatException(string message) : base(message)
        {
        }

        public FormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
