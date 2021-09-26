using System;

namespace Gint.Pipes
{
    public class PipeOperationException : Exception
    {
        public PipeOperationException(string message) : base(message)
        {
        }

        public PipeOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
