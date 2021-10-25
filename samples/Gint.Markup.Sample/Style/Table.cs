using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        Columns = new[] { new Column { Content = "header1" }, new Column { Content = "header2.." }, new Column { Content = "header3.." } }
                    }
                },
                Content = new Content
                {
                    Rows = new[]
                    {
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column { Content = "content12.xxxx." }, new Column { Content = "content13.xx." } }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column { Content = "content12.xxxx." }, new Column { Content = "content13.xx." } }
                        },
                         new Row
                        {
                            Columns = new[] { new Column { Content = "content21.." , TotalColumnsWidth = 2 }, new Column { Content = "header23.." } }
                        },
                    }
                }
            };

            new TableRenderer(table, Console.Out).Render();
        }
    }

    public class TableRenderer
    {
        public Table Table { get; }
        public TableRenderOptions TableRenderOptions { get; }
        public TextWriter writer;

        public TableRenderer(Table table, TextWriter writer)
        {
            Table = table;
            this.writer = writer;
            TableRenderOptions = new TableRenderOptions(table.LargestCellSize, table.Header.Row.Columns.Length);
        }

        public void Render()
        {
            RenderHeaderTopFrameSection(isFirst: true);
            for (int i = 1; i < TableRenderOptions.TotalColumns - 1; i++)
            {
                RenderHeaderTopFrameSection();
            }
            RenderHeaderTopFrameSection(isLast: true);
            RenderHeaderFrameStart();
            for (int i = 0; i < Table.Header.Row.Columns.Length; i++)
            {
                Column column = Table.Header.Row.Columns[i];
                RenderHeaderColumn(column);

                if (i < Table.Header.Row.Columns.Length - 1)
                    RenderHeaderColumnDivider();
            }
            RenderHeaderFrameEnd();

            RenderHeaderBottomFrameSection(isFirst: true);
            for (int i = 1; i < TableRenderOptions.TotalColumns - 1; i++)
            {
                RenderHeaderBottomFrameSection();
            }
            RenderHeaderBottomFrameSection(isLast: true);
            ChangeLine();

            for (int j = 0; j < Table.Content.Rows.Length; j++)
            {
                Row row = Table.Content.Rows[j];
                RenderContentFrameStart();
                for (int i = 0; i < row.Columns.Length; i++)
                {
                    Column column = row.Columns[i];
                    RenderContentColumn(column);

                    if (i < row.Columns.Length - 1)
                        RenderContentColumnDivider();
                }
                RenderContentFrameEnd();

                if (j < Table.Content.Rows.Length - 1)
                {
                    RenderContentBottomFrameSection(isFirst: true);
                    for (int i = 1; i < TableRenderOptions.TotalColumns - 1; i++)
                    {
                        RenderContentBottomFrameSection();
                    }
                    RenderContentBottomFrameSection(isLast: true);
                    ChangeLine();
                }
            }
            RenderContentBottomBoxSection(isFirst: true);
            for (int i = 0; i < Table.Content.Rows.Length - 2; i++)
            {
                Row row = Table.Content.Rows[i];
                RenderContentBottomBoxSection();
            }
            RenderContentBottomBoxSection(isLast: true);
        }


        public void ChangeLine()
        {
            writer.WriteLine();
        }

        #region Header  

        public void RenderHeaderTopFrameSection(bool isFirst = false, bool isLast = false)
        {
            if (isFirst)
                writer.Write('┌');
            writer.Write(new string('─', TableRenderOptions.TotalWidthWithoutMargin));
            if (isLast)
                writer.Write('┐');
            else
                writer.Write('─');
        }

        public void RenderHeaderBottomFrameSection(bool isFirst = false, bool isLast = false)
        {
            if (isFirst)
                writer.Write('└');
            writer.Write(new string('─', TableRenderOptions.TotalWidthWithoutMargin));
            if (isLast)
                writer.Write('┘');
            else
                writer.Write('─');
        }

        public void RenderHeaderColumnDivider()
        {
            writer.Write('│');
        }

        public void RenderHeaderFrameStart()
        {
            ChangeLine();
            writer.Write('│');
        }

        public void RenderHeaderFrameEnd()
        {
            writer.Write('│');
            ChangeLine();
        }

        public void RenderHeaderColumn(Column column)
        {
            RenderColumn(column, column.Alignment == Alignment.Default ? TableRenderOptions.DefaultContentAlignment : column.Alignment);
        }

        #endregion

        #region Content

        public void RenderContentBottomFrameSection(bool isFirst = false, bool isLast = false)
        {
            if (isFirst)
                writer.Write('│');
            writer.Write(new string('─', TableRenderOptions.TotalWidthWithoutMargin));
            if (isLast)
                writer.Write('│');
            else
                writer.Write('─');
        }

        public void RenderContentBottomBoxSection(bool isFirst = false, bool isLast = false)
        {
            if (isFirst)
                writer.Write('└');
            writer.Write(new string('─', TableRenderOptions.TotalWidthWithoutMargin));
            if (isLast)
                writer.Write('┘');
            else
                writer.Write('─');
        }

        public void RenderContentFrameStart()
        {
            writer.Write('|');
        }

        public void RenderContentFrameEnd()
        {
            writer.Write('|');
            ChangeLine();
        }

        private void RenderContentColumnDivider()
        {
            writer.Write('|');
        }

        public void RenderContentColumn(Column column)
        {
            RenderColumn(column, column.Alignment == Alignment.Default ? TableRenderOptions.DefaultContentAlignment : column.Alignment);
        }

        private void RenderColumn(Column column, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Start:
                    RenderStart(column);
                    break;
                case Alignment.Center:
                    RenderCenter(column);
                    break;
                case Alignment.End:
                    RenderEnd(column);
                    break;
            }

        }

        private void RenderStart(Column column)
        {
            writer.Write(new string(' ', TableRenderOptions.PaddingLeft));
            writer.Write(column.Content);
            var space = (column.TotalColumnsWidth * TableRenderOptions.CellSize) - column.Content.Length;
            if (space > 0)
                writer.Write(new string(' ', space));
            writer.Write(new string(' ', TableRenderOptions.PaddingRight * column.TotalColumnsWidth));

            if (column.TotalColumnsWidth > 1)
            {
                writer.Write(new string(' ', (column.TotalColumnsWidth - 1) * TableRenderOptions.ColumnDividerWidth));
                writer.Write(new string(' ', (column.TotalColumnsWidth - 1) * TableRenderOptions.PaddingLeft));
            }
        }

        private void RenderEnd(Column column)
        {
            var totalWidth = TableRenderOptions.CellSize * column.TotalColumnsWidth
                + ((column.TotalColumnsWidth - 1) * TableRenderOptions.ColumnDividerWidth)
                + ((column.TotalColumnsWidth) * TableRenderOptions.PaddingLeft)
                + ((column.TotalColumnsWidth - 1) * TableRenderOptions.PaddingRight);

            var offset = totalWidth - column.Content.Length;
            if (offset > 0)
            {
                writer.Write(new string(' ', offset));
            }

            writer.Write(column.Content);
            writer.Write(new string(' ', TableRenderOptions.PaddingRight));
        }

        private void RenderCenter(Column column)
        {
            var totalWidth = TableRenderOptions.CellSize * column.TotalColumnsWidth
                + ((column.TotalColumnsWidth - 1) * TableRenderOptions.ColumnDividerWidth)
                + ((column.TotalColumnsWidth - 1) * TableRenderOptions.PaddingLeft)
                + ((column.TotalColumnsWidth - 1) * TableRenderOptions.PaddingRight);

            double offset = (totalWidth - column.Content.Length) / 2;

            writer.Write(new string(' ', TableRenderOptions.PaddingLeft + (int)Math.Ceiling(offset)));
            writer.Write(column.Content);
            var space = totalWidth - column.Content.Length - (int)Math.Floor(offset);
            if (space > 0)
                writer.Write(new string(' ', space));
            writer.Write(new string(' ', TableRenderOptions.PaddingRight));

        }

        #endregion


    }
    public enum Alignment
    {
        Default,
        Start,
        Center,
        End
    }

    public class Table
    {
        public Header Header { get; init; }
        public Content Content { get; init; }

        public IEnumerable<Column> IterateColumns => Header.Row.Columns.Concat(Content.Rows.SelectMany(c => c.Columns));

        internal int LargestCellSize => IterateColumns.Max(c => c.Content.Length / c.TotalColumnsWidth);

        public bool Validate(bool beForgiving)
        {
            throw new NotImplementedException();
        }
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
        public int TotalColumnsWidth { get; init; } = 1;
        public string Content { get; init; }
        public Alignment Alignment { get; init; } = Alignment.Default;
    }

}
