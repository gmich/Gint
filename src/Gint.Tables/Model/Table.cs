using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gint.Tables
{
    public class Table
    {
        [JsonInclude]
        public Header Header { get; internal set; }

        [JsonInclude]
        public Content Content { get; internal set; }

        [JsonIgnore]
        internal bool HasHeader => Header != null;

        [JsonIgnore]
        internal IEnumerable<Column> IterateColumns => (Header?.Rows.SelectMany(c => c.Columns) ?? new Column[0]).Concat(Content.Rows.SelectMany(c => c.Columns));

        [JsonIgnore]
        internal IEnumerable<Column> IterateFirstRow => Header?.Rows.SelectMany(c => c.Columns) ?? Content.Rows.FirstOrDefault().Columns;
    }
}
