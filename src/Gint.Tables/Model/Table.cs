using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Tables
{
    internal class Table
    {
        public Header Header { get; internal set; }
        public Content Content { get; internal set; }

        internal bool HasHeader => Header != null;
        internal IEnumerable<Column> IterateColumns => (Header?.Rows.SelectMany(c => c.Columns) ?? new Column[0]).Concat(Content.Rows.SelectMany(c => c.Columns));
        internal IEnumerable<Column> IterateFirstRow => Header?.Rows.SelectMany(c => c.Columns) ?? Content.Rows.FirstOrDefault().Columns;
    }
}
