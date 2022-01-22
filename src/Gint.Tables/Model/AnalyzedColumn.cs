using System.Text.Json.Serialization;

namespace Gint.Tables
{
    public struct AnalyzedColumn
    {
        public AnalyzedColumn(Column column, bool skipColumnDivider, int totalCells)
        {
            Column = column;
            SkipColumnDivider = skipColumnDivider;
            TotalCells = totalCells;
        }

        public Column Column { get; }

        [JsonIgnore]
        public bool SkipRowDivider => Column.SkipRowDivider;

        public bool SkipColumnDivider { get; }
        public int TotalCells { get; }
    }
}
