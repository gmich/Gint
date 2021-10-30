using System.Collections.Generic;
using System.Linq;

namespace Gint.Markup.Sample
{
    public class FluentRowBuilder
    {
        private Alignment _alignment = Alignment.Default;
        private bool? _withRowDivider = null;
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

        public FluentColumnBuilder AddColumn(string content)
        {
            return new FluentColumnBuilder(_rows, new RowContext(_alignment, _withRowDivider, isHeader), content);
        }

        public FluentRowBuilder WithRowDivider()
        {
            _withRowDivider = false;
            return this;
        }

        public FluentRowBuilder WithoutRowDivider()
        {
            _withRowDivider = true;
            return this;
        }

        public FluentRowBuilder WithRowAlignment(Alignment alignment)
        {
            _alignment = alignment;
            return this;
        }

        public ITableRenderer Build()
        {
            var table = new Table();
            table.Content = new Content();

            if (_rows.Count == 0)
            {
                //no table
            }

            var maxColumnsInRow = _rows.Max(c => c.Columns.Sum(c => c.SpansOverColumns));

            table.Header = _rows.Where(c => c.IsHeader).Select(c =>
            new Header
            {
                Row = new Row
                {
                    Alignment = c.Alignment,
                    Columns = c.Columns.ToArray()
                }
            }).FirstOrDefault();

            table.Content.Rows = _rows.Where(c => !c.IsHeader).Select(c => new Row
            {
                Alignment = c.Alignment,
                Columns = c.Columns.ToArray()
            }).ToArray();

            //normalize header column length
            if(table.Header!=null)
            {
                var totalColumns = table.Header.TotalColumns;
                if(totalColumns< maxColumnsInRow)
                {
                    table.Header.Row.Columns.Last().SpansOverColumns += (maxColumnsInRow - totalColumns);
                }
            }

            //normalize content column length
            foreach(var row in table.Content.Rows)
            {
                var totalColumns = row.TotalColumns;
                if (totalColumns < maxColumnsInRow)
                {
                    row.Columns.Last().SpansOverColumns += (maxColumnsInRow - totalColumns);
                }
            }

            return new TableRenderer(table, GintTable.TableRenderPreferences);
        }
    }
}
