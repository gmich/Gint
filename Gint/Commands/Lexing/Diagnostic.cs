namespace Gint
{
    internal sealed class Diagnostic
    {
        private Diagnostic(bool isError, TextSpan location, string message)
        {
            IsError = isError;
            Location = location;
            Message = message;
            IsWarning = !IsError;
        }

        public bool IsError { get; }
        public TextSpan Location { get; }
        public string Message { get; }
        public bool IsWarning { get; }

        public override string ToString() => Message;

        public static Diagnostic Error(TextSpan location, string message)
        {
            return new Diagnostic(isError: true, location, message);
        }

        public static Diagnostic Warning(TextSpan location, string message)
        {
            return new Diagnostic(isError: false, location, message);
        }
    }
}
