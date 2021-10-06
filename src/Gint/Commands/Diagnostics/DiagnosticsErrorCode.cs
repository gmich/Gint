namespace Gint
{
    public static class DiagnosticsErrorCode
    {
        public const string UnterminatedString = "G001";
        public const string UnexpectedToken = "G002";
        public const string NullCommand = "G012";
        public const string CommandUnknown = "G003";
        public const string OptionUnknown = "G004";
        public const string NullOption = "G014";
        public const string UnecessaryVariable = "G005";
        public const string MissingVariable = "G006";
        public const string UnterminatedPipeline = "G007";
        public const string MissingCommandToPipe = "G008";
        public const string CommandIsNotACommandWithVariable = "G009";
        public const string CommandHasRequiredVariable = "G010";
        public const string MultipleOptionsNotAllowed = "G011";
    }
}
