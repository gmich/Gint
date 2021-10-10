using System;
using System.Collections.Generic;

namespace Gint.SyntaxHighlighting
{
    public class NoopHistoryRepository : ICommandHistoryRepository
    {
        public void Add(string entry) { }
        public IEnumerable<string> Load() => new List<string>();
    }
}
