using System;

namespace Gint
{
    public class CommandExecutionEventArgs : EventArgs
    {
        public GlobalExecutionContext GlobalExecutionContext { get; }

        public CommandExecutionEventArgs(GlobalExecutionContext commandExecutionContext)
        {
            GlobalExecutionContext = commandExecutionContext;
        }
    }
}
