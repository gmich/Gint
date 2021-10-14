namespace Gint
{
    public class Command
    {
        public Command(string commandName, HelpCallback helpCallback, ExecutionBlock callback, SuggestionsCallback suggestions = null)
        {
            CommandName = commandName;
            HelpCallback = helpCallback;
            Callback = callback;
            Suggestions = suggestions ?? CallbackUtilities.EmptySuggestions;
        }

        public string CommandName { get; }
        public HelpCallback HelpCallback { get; }
        public ExecutionBlock Callback { get; }
        public SuggestionsCallback Suggestions { get; }

        public override string ToString() => CommandName;

    }
}
