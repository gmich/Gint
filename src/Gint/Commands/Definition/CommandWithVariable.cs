namespace Gint
{
    public class CommandWithVariable : Command
    {
        public CommandWithVariable(string commandName, bool required, HelpCallback helpCallback, ExecutionBlock callback, SuggestionsCallback suggestions = null) : base(commandName, helpCallback, callback, suggestions)
        {
            Required = required;
        }

        public bool Required { get; }
        public override string ToString() => CommandName;

    }
}
