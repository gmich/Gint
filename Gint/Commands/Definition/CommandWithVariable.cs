namespace Gint
{
    public class CommandWithVariable : Command
    {
        public CommandWithVariable(string commandName, bool required, HelpCallback helpCallback, ExecutionBlock callback) : base(commandName, helpCallback, callback)
        {
            Required = required;
        }

        public bool Required { get; }
        public override string ToString() => CommandName;

    }
}
