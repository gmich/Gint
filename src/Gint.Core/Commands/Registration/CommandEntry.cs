using System.Linq;

namespace Gint
{
    public class CommandEntry
    {
        public CommandEntry(Command command, Option[] options)
        {
            Command = command;
            Options = options;
        }

        public Command Command { get; }
        public Option[] Options { get; internal set; }

    }
}
