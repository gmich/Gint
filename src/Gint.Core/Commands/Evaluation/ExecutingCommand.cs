namespace Gint
{
    public class ExecutingCommand
    {
        public ExecutingCommand(int executionId,string fullCommand, string variable, string[] options, TextSpan executionSpan)
        {
            ExecutionId = executionId;
            FullCommand = fullCommand;
            Variable = variable;
            Options = options;
            ExecutionTextspan = executionSpan;
        }

        public int ExecutionId { get; }
        public string FullCommand { get; }
        public string Variable { get; }
        public string[] Options { get; }
        public TextSpan ExecutionTextspan { get; }
        public string ExecutionSpan => ExecutionTextspan.GetText(FullCommand);

        public bool HasVariable => !string.IsNullOrEmpty(Variable);


    }

}
