namespace Gint
{
    internal class CommandInput : ICommandInput
    {
        public CommandInput(int executionId, string variables, string[] options, CommandScope scope)
        {
            Variable = variables;
            Options = options;
            Scope = scope;
            ExecutionId = executionId;
        }

        public int ExecutionId { get; }
        public string Variable { get; }
        public string[] Options { get; }
        public CommandScope Scope { get; }

        public static ICommandInput Empty { get; } = new CommandInput(0, string.Empty, System.Array.Empty<string>(), new CommandScope(null, null));
    }
}
