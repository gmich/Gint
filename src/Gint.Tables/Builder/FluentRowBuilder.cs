using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.Tables
{
    public class FluentRowBuilder
    {
        private Alignment _alignment = Alignment.Default;
        private bool? _skipRowDivider = null;
        private List<RowContext> _rows;
        private readonly bool isHeader;

        internal FluentRowBuilder(List<RowContext> previousRows, RowContext newRow = null, bool isHeader = false)
        {
            _rows = previousRows;
            this.isHeader = isHeader;
            if (newRow != null)
            {
                _rows.Add(newRow);
            }
        }
        public FluentRowBuilder RenderEmptyRow()
        {
            var builder = AddColumn(" ").NewRow();
            builder._rows.Last().IsEmpty = true;
            return builder;
        }

        public FluentColumnBuilder AddColumn(string content)
        {
            return new FluentColumnBuilder(_rows, new RowContext(_alignment, _skipRowDivider, isHeader), content);
        }

        public FluentRowBuilder WithRowDivider()
        {
            _skipRowDivider = false;
            return this;
        }

        public FluentRowBuilder WithoutRowDivider()
        {
            _skipRowDivider = true;
            return this;
        }

        public FluentRowBuilder WithRowAlignment(Alignment alignment)
        {
            _alignment = alignment;
            return this;
        }

        public TableDefinition Build(TableRenderPreferences tableRenderPreferences = null)
        {
            var table = new Table();
            table.Content = new Content();

            var maxColumnsInRow = _rows.Max(c => c.Columns.Sum(c => c.SpansOverColumns));

            table.Header = _rows.Where(c => c.IsHeader).Select(c =>
            new Header
            {
                Rows = new[]
                {
                    new Row
                    {
                        SkipDivider = c.SkipRowDivider ?? false,
                        Alignment = c.Alignment,
                        Columns = c.Columns.ToArray()
                    }
                }
            }).FirstOrDefault();

            table.Content.Rows = _rows.Where(c => !c.IsHeader).Select(c => new Row
            {
                SkipColumns = c.IsEmpty,
                SkipDivider = c.SkipRowDivider ?? false,
                Alignment = c.Alignment,
                Columns = c.Columns.ToArray()
            }).ToArray();

            void NormalizeColumnLength(Row[] rows)
            {
                foreach (var row in rows)
                {
                    var totalColumns = row.TotalColumns;
                    if (totalColumns < maxColumnsInRow)
                    {
                        row.Columns.Last().SpansOverColumns += (maxColumnsInRow - totalColumns);
                    }
                }
            }
            NormalizeColumnLength(table.Content.Rows);
            NormalizeColumnLength(table.Header.Rows);

            return new TableDefinition
            {
                TableRenderPreferences = tableRenderPreferences ?? GintTable.DefaultTableRenderPreferences,
                Table = table
            };
        }
    }
}
