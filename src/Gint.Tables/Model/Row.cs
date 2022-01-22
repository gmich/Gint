using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Gint.Tables
{
    public class Row
    {
        [JsonInclude]
        public bool SkipColumns { get; internal set; } = false;

        [JsonInclude]
        public bool SkipDivider { get; internal set; } = false;

        [JsonInclude]
        public Alignment Alignment { get; internal set; } = Alignment.Default;

        [JsonInclude]
        public Column[] Columns { get; internal set; }

        [JsonIgnore]
        public List<AnalyzedColumn> AnalyzedColumns { get; set; }

        [JsonIgnore]
        internal int TotalColumns => Columns.Sum(c => c.SpansOverColumns);
    }
}
