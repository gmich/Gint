using System.Collections.Generic;

namespace Gint.Terminal
{
    public interface ICommandHistoryRepository
    {
        IEnumerable<string> Load();
        void Add(string entry); 
    }
}
