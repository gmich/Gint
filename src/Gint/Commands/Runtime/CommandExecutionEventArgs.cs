using System;

namespace Gint
{
    public class CommandExecutionEventArgs : EventArgs
    {
        public CommandExecutionContext CommandExecutionContext { get; }

        public CommandExecutionEventArgs(CommandExecutionContext commandExecutionContext)
        {
            CommandExecutionContext = commandExecutionContext;
        }
    }
}
