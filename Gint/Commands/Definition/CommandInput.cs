namespace Gint
{
    internal class CommandInput : ICommandInput
    {
        public CommandInput(int executionId, string variables, string stream)
        {
            Variable = variables;
            Stream = stream;
            ExecutionId = executionId;
        }

        public int ExecutionId { get; }
        public string Variable { get; }
        public string Stream { get; }

        public static ICommandInput Empty { get; } = new CommandInput(0, string.Empty,string.Empty);
    }
}
