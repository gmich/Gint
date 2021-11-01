using System.Collections.Generic;
using System.Linq;

namespace Gint.Tables
{
    internal class Row
    {
        public bool SkipColumns { get; set; } = false;
        public bool SkipDivider { get; set; } = false;
        public Alignment Alignment { get; init; } = Alignment.Default;
        public Column[] Columns { get; init; }
        public List<AnalyzedColumn> AnalyzedColumns { get; internal set; }
        internal int TotalColumns => Columns.Sum(c => c.SpansOverColumns);
    }
}
