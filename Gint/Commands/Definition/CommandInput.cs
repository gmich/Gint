namespace Gint
{
    internal class CommandInput : ICommandInput
    {
        public CommandInput(int executionId, string variables, InputStream stream, string[] options)
        {
            Variable = variables;
            Stream = stream;
            Options = options;
            ExecutionId = executionId;
        }

        public int ExecutionId { get; }
        public string Variable { get; }
        public InputStream Stream { get; }
        public string[] Options { get; }

        public static ICommandInput Empty { get; } = new CommandInput(0, string.Empty,new InputStream(string.Empty,string.Empty), System.Array.Empty<string>());
    }
}
