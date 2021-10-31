﻿using System;
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

    internal class TableAnalyzer
    {
        public static void AdjustTable(Table table, int maxCellSize)
        {
            if(table.HasHeader)
                table.Header.Rows = AdjustRows(table.Header.Rows, maxCellSize);

            table.Content.Rows = AdjustRows(table.Content.Rows, maxCellSize);
        }

        public static Row[] AdjustRows(Row[] rows, int maxCellSize)
        {
            List<Row> newRows = new List<Row>();
            Row rowToAnalyze = null;
            bool addExtraRow;

            foreach (var row in rows)
            {
                rowToAnalyze = row;
                newRows.Add(row);
                var originalSkipDivider = row.SkipDivider;
                do
                {
                    (bool AddExtraRow, List<Column> ExtraColumns) = AnalyzeRow(rowToAnalyze, maxCellSize);
                    addExtraRow = AddExtraRow;
                    if (addExtraRow)
                    {
                        rowToAnalyze.SkipDivider = true;

                        var newRow = new Row
                        {
                            Columns = ExtraColumns.ToArray(),
                            SkipDivider = false,
                            Alignment = row.Alignment
                        };
                        rowToAnalyze = newRow;
                        newRows.Add(newRow);
                    }

                } while (addExtraRow);
                newRows.Last().SkipDivider = originalSkipDivider;
            }

            return newRows.ToArray();
        }
        private static (bool AddExtraRow, List<Column> ExtraColumns) AnalyzeRow(Row row, int maxCellSize)
        {
            bool addExtraRow = false;
            List<Column> extraColumns = new List<Column>();

            foreach (var column in row.Columns)
            {
                extraColumns.Add(new Column
                {
                    SpansOverColumns = column.SpansOverColumns,
                    SkipRowDivider = column.SkipRowDivider,
                    Alignment = column.Alignment,
                });
                if(column.Content.Length > maxCellSize)
                {
                    column.Content= column.Content.Insert(maxCellSize, Environment.NewLine);
                }

                var newContent = column.Content.Split(Environment.NewLine);

                if (newContent.Length < 2)
                {
                    continue;
                }
                else
                {
                    addExtraRow = true;
                    column.Content = newContent[0];
                    extraColumns.Last().Content = string.Join(Environment.NewLine, newContent.Skip(1));
                }

            }
            return (addExtraRow, extraColumns);
        }
    }
}
