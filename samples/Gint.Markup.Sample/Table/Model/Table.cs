using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Markup.Sample
{
    internal class Table
    {
        public Header Header { get; init; }
        public Content Content { get; init; }

        internal IEnumerable<Column> IterateColumns => (Header?.Row.Columns ?? new Column[0]).Concat(Content.Rows.SelectMany(c => c.Columns));
        internal IEnumerable<Column> IterateFirstRow => Header?.Row.Columns ?? Content.Rows.FirstOrDefault().Columns;
    }

    internal class Content
    {
        public Alignment Alignment { get; set; } = Alignment.Default;
        public Row[] Rows { get; init; }
    }

    internal class Header
    {
        public Alignment Alignment { get; set; } = Alignment.Default;
        public Row Row { get; init; }
    }

    internal class Row
    {
        public Alignment Alignment { get; init; } = Alignment.Default;
        public Column[] Columns { get; init; }
        public List<AnalyzedColumn> AnalyzedColumns { get; internal set; }

    }

    internal struct AnalyzedColumn
    {
        public AnalyzedColumn(Column column, bool skipColumnDivider, int totalCells)
        {
            Column = column;
            SkipColumnDivider = skipColumnDivider;
            TotalCells = totalCells;
        }

        public Column Column { get; }
        public bool SkipRowDivider => Column.SkipRowDivider;
        public bool SkipColumnDivider { get; }
        public int TotalCells { get; }
    }

    internal class Column
    {
        public int SpansOverColumns { get; init; } = 1;
        public string Content { get; init; } = string.Empty;
        public Alignment Alignment { get; init; } = Alignment.Default;
        internal string Rendered { get; set; }
        public bool SkipRowDivider { get; set; }
    }

}
