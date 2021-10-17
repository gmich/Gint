using System.Threading.Tasks;

namespace Gint
{
    public struct CommandResult
    {
        internal CommandResult(CommandState state)
        {
            CommandState = state;
        }

        public CommandState CommandState { get; }

        public static Task<CommandResult> SuccessfulTask => Task.FromResult(new CommandResult(CommandState.Success));
        public static Task<CommandResult> ErrorTask => Task.FromResult(new CommandResult(CommandState.Error));

        public static CommandResult Success => new CommandResult(CommandState.Success);
        public static CommandResult Error => new CommandResult(CommandState.Error);
    }
}
