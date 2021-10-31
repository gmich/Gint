using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Markup.Sample
{
    internal class Table
    {
        public Header Header { get; internal set; }
        public Content Content { get; internal set; }

        internal bool HasHeader => Header != null;
        internal IEnumerable<Column> IterateColumns => (Header?.Rows.SelectMany(c => c.Columns) ?? new Column[0]).Concat(Content.Rows.SelectMany(c => c.Columns));
        internal IEnumerable<Column> IterateFirstRow => Header?.Rows.SelectMany(c => c.Columns) ?? Content.Rows.FirstOrDefault().Columns;
    }

    internal class Content
    {
        public Row[] Rows { get; internal set; }
    }

    internal class Header
    {
        public Row[] Rows { get; internal set; }
    }

    internal class Row
    {
        public bool SkipColumns { get; set; } = false;
        public bool SkipDivider { get; set; } = false;
        public Alignment Alignment { get; init; } = Alignment.Default;
        public Column[] Columns { get; init; }
        public List<AnalyzedColumn> AnalyzedColumns { get; internal set; }
        internal int TotalColumns => Columns.Sum(c => c.SpansOverColumns);
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
        internal int SpansOverColumns { get; set; } = 1;
        internal string Content { get; set; } = string.Empty;
        internal Alignment Alignment { get; init; } = Alignment.Default;
        internal string Rendered { get; set; }
        internal bool SkipRowDivider { get; set; }

        internal int ContentLength => Content.Split(Environment.NewLine).Max(c => c.Length);
    }
}
