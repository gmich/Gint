namespace Gint
{
    internal class CommandInput : ICommandInput
    {
        public CommandInput(int executionId, string variables, InputStream stream, string[] options, CommandScope scope)
        {
            Variable = variables;
            Stream = stream;
            Options = options;
            Scope = scope;
            ExecutionId = executionId;
        }

        public int ExecutionId { get; }
        public string Variable { get; }
        public InputStream Stream { get; }
        public string[] Options { get; }
        public CommandScope Scope { get; }

        public static ICommandInput Empty { get; } = new CommandInput(0, string.Empty,new InputStream(string.Empty,string.Empty), System.Array.Empty<string>(), new CommandScope());
    }
}
