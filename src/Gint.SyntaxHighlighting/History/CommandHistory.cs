using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.SyntaxHighlighting
{
    public interface ICommandHistoryRepository
    {
        IEnumerable<string> Load();
        void Add(string entry); 
    }

    public class CommandHistory
    {
        private readonly List<string> entries;
        private readonly ICommandHistoryRepository commandHistoryRepository;
        private int lastLookup = 0;

        public CommandHistory(ICommandHistoryRepository commandHistoryRepository)
            : base()
        {
            this.commandHistoryRepository = commandHistoryRepository;
            entries = commandHistoryRepository.Load().ToList();
            lastLookup = entries.Count;
        }

        private void InternalAdd(string history)
        {
            entries.Add(history);
            commandHistoryRepository.Add(history);
            lastLookup = entries.Count;
        }

        public void Record(string history)
        {
            InternalAdd(history);
        }

        internal bool GetPrevious(out string command)
        {
            if (lastLookup > 0)
            {
                lastLookup--;
                command = entries[lastLookup];
                return true;
            }
            command = default;
            return false;
        }

        internal bool GetNext(out string command)
        {
            if (entries.Count > lastLookup + 1)
            {
                lastLookup++;
                command = entries[lastLookup];
                return true;
            }
            command = string.Empty;
            return true;
        }
    }
}
