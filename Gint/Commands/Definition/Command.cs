namespace Gint
{
    public class Command
    {
        public Command(string commandName, HelpCallback helpCallback, ExecutionBlock callback)
        {
            CommandName = commandName;
            HelpCallback = helpCallback;
            Callback = callback;
        }

        public string CommandName { get; }
        public HelpCallback HelpCallback { get; }
        public ExecutionBlock Callback { get; }

        public override string ToString() => CommandName;

    }
}
