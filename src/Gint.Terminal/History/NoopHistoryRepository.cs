using System;
using System.Collections.Generic;

namespace Gint.Terminal
{
    public class NoopHistoryRepository : ICommandHistoryRepository
    {
        public void Add(string entry) { }
        public IEnumerable<string> Load() => new List<string>();
    }
}
