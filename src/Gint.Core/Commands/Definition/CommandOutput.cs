using System.Threading.Tasks;

namespace Gint
{
    public struct CommandOutput
    {
        internal CommandOutput(CommandState state)
        {
            CommandState = state;
        }

        public CommandState CommandState { get; }

        public static Task<CommandOutput> SuccessfulTask => Task.FromResult(new CommandOutput(CommandState.Success));
        public static Task<CommandOutput> ErrorTask => Task.FromResult(new CommandOutput(CommandState.Error));

        public static CommandOutput Success => new CommandOutput(CommandState.Success);
        public static CommandOutput Error => new CommandOutput(CommandState.Error);
    }
}
