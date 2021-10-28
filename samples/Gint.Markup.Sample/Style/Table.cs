using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gint.Markup.Sample
{
    public class TablePoc
    {
        public TablePoc()
        {
            var table = new Table
            {
                Header = new Header
                {
                    Row = new Row
                    {
                        Columns = new[] { new Column { Content = "header1", SpansOverColumns = 2 }, new Column { Content = "header2..", SkipRowDivider=true }, new Column { Content = "header3.." } }
                    }
                },
                Content = new Content
                {
                    Rows = new[]
                    {
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column { Content = "content12.xxxx." }, new Column { Content = "content13.xx." }, new Column { SkipRowDivider=true } }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column {  }, new Column {  }, new Column {} }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column { Content = "content12.xxxx." }, new Column { Content = "content13.xx." }, new Column {  } }
                        },
                         new Row
                        {
                            Columns = new[] { new Column { Content = "content21.." , SpansOverColumns = 2 }, new Column { Content = "header23.." }, new Column {  } }
                        },
                    }
                }
            };

            new TableRenderer(table, Console.Out, new TableRenderPreferences()).Render();
        }
    }

    public class Table
    {
        public Header Header { get; init; }
        public Content Content { get; init; }

        internal IEnumerable<Column> IterateColumns => Header.Row.Columns.Concat(Content.Rows.SelectMany(c => c.Columns));
        internal IEnumerable<Row> IterateRows => new[] { Header.Row }.Concat(Content.Rows);
    }

    public class Content
    {
        public Alignment Alignment { get; set; } = Alignment.Default;
        public Row[] Rows { get; init; }
    }

    public class Header
    {
        public Alignment Alignment { get; set; } = Alignment.Default;
        public Row Row { get; init; }
    }

    public class Row
    {
        public Alignment Alignment { get; init; } = Alignment.Default;
        public Column[] Columns { get; init; }
    }

    public class Column
    {
        public int SpansOverColumns { get; init; } = 1;
        public string Content { get; init; } = string.Empty;
        public Alignment Alignment { get; init; } = Alignment.Default;
        internal string Rendered { get; set; }
        public bool SkipRowDivider { get; set; }
    }

}
