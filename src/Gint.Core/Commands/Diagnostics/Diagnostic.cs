namespace Gint
{
    internal sealed class Diagnostic
    {
        private Diagnostic(string errorCode, bool isError, TextSpan location, string message)
        {
            ErrorCode = errorCode;
            IsError = isError;
            Location = location;
            Message = message;
            IsWarning = !IsError;
        }

        public string ErrorCode { get; }
        public bool IsError { get; }
        public TextSpan Location { get; }
        public string Message { get; }
        public bool IsWarning { get; }

        public override string ToString() => Message;

        public static Diagnostic Error(string errorCode, TextSpan location, string message)
        {
            return new Diagnostic(errorCode, isError: true, location, message);
        }

        public static Diagnostic Warning(string errorCode, TextSpan location, string message)
        {
            return new Diagnostic(errorCode, isError: false, location, message);
        }
    }
}
