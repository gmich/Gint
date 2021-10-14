using System.Threading.Tasks;

namespace Gint
{
    public struct CommandOutput : ICommandOutput
    {
        public CommandOutput(CommandState state)
        {
            CommandState = state;
        }

        public CommandState CommandState { get; }

        public static Task<ICommandOutput> SuccessfulTask => Task.FromResult<ICommandOutput>(new CommandOutput(CommandState.Success));
        public static Task<ICommandOutput> ErrorTask => Task.FromResult<ICommandOutput>(new CommandOutput(CommandState.Error));

        public static CommandOutput Success => new CommandOutput(CommandState.Success);
        public static CommandOutput Error => new CommandOutput(CommandState.Error);
    }
}
