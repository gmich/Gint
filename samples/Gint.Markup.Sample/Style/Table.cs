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
                            Columns = new[] { new Column { Content = "content21.." }, new Column { Content = "header22.." }, new Column { Content = "header23.." } }
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
            for (int i = 1; i < TableRenderOptions.TotalColumns; i++)
            {
                RenderHeaderTopFrameSection();
            }
            RenderHeaderFrameStart();
            for (int i = 0; i < Table.Header.Row.Columns.Length; i++)
            {
                Column column = Table.Header.Row.Columns[i];
                RenderHeaderColumn(column.Content, column.Alignment);

                if (i < Table.Header.Row.Columns.Length - 1)
                    RenderHeaderColumnDivider();
            }
            RenderHeaderFrameEnd();
        
            RenderHeaderBottomFrameSection(isFirst: true);
            for (int i = 1; i < TableRenderOptions.TotalColumns; i++)
            {
                RenderHeaderBottomFrameSection();
            }
            ChangeLine();

            foreach (var row in Table.Content.Rows)
            {
                RenderContentFrameStart();
                for (int i = 0; i < row.Columns.Length; i++)
                {
                    Column column = row.Columns[i];
                    RenderContentColumn(column.Content, column.Alignment);

                    if (i < row.Columns.Length - 1)
                        RenderContentColumnDivider();
                }
                RenderContentFrameEnd();
            
                RenderContentBottomFrameSection(isFirst: true);
                for (int i = 1; i < TableRenderOptions.TotalColumns; i++)
                {
                    RenderContentBottomFrameSection();
                }
                ChangeLine();
            }
        }


        public void ChangeLine()
        {
            writer.WriteLine();
        }

        #region Header

        public void RenderHeaderTopFrameSection(bool isFirst = false)
        {
            if (isFirst)
                writer.Write(' ');
            writer.Write(new string(' ', TableRenderOptions.TotalWidthWithoutMargin));
            writer.Write(' ');
        }

        public void RenderHeaderBottomFrameSection(bool isFirst = false)
        {
            if (isFirst)
                writer.Write('+');
            writer.Write(new string('-', TableRenderOptions.TotalWidthWithoutMargin));
            writer.Write('+');
        }

        public void RenderHeaderColumnDivider()
        {
            writer.Write('|');
        }

        public void RenderHeaderColumnEnd()
        {
            writer.Write('|');
        }

        public void RenderHeaderFrameStart()
        {
            ChangeLine();
            writer.Write('|');
        }

        public void RenderHeaderFrameEnd()
        {
            writer.Write('|');
            ChangeLine();
        }

        public void RenderHeaderColumn(string content, Alignment alignment)
        {
            //Alignment.Center
            var offset = (TableRenderOptions.CellSize - content.Length) / 2;

            writer.Write(new string(' ', TableRenderOptions.PaddingLeft + offset));
            writer.Write(content);
            var space = TableRenderOptions.CellSize - content.Length - offset;
            if (space > 0)
                writer.Write(new string(' ', space));
            writer.Write(new string(' ', TableRenderOptions.PaddingRight));
        }

        #endregion

        #region Content

        public void RenderContentBottomFrameSection(bool isFirst = false)
        {
            if (isFirst)
                writer.Write('+');
            writer.Write(new string('-', TableRenderOptions.TotalWidthWithoutMargin));
            writer.Write('+');
        }

        public void RenderContentColumnEnd()
        {
            writer.Write('|');
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

        public void RenderContentColumn(string content, Alignment alignment)
        {
            //Alignment.Start
            writer.Write(new string(' ', TableRenderOptions.PaddingLeft));
            writer.Write(content);
            var space = TableRenderOptions.CellSize - content.Length;
            if (space > 0)
                writer.Write(new string(' ', space));
            writer.Write(new string(' ', TableRenderOptions.PaddingRight));
        }

        #endregion


    }
    public enum Alignment
    {
        Inherit,
        Start,
        Center,
        End
    }

    public class Table
    {
        public Alignment Alignment { get; set; } = Alignment.Center;
        public Header Header { get; init; }
        public Content Content { get; init; }

        public IEnumerable<Column> IterateColumns => Header.Row.Columns.Concat(Content.Rows.SelectMany(c => c.Columns));

        internal int LargestCellSize => IterateColumns.Max(c => c.Content.Length);

        public bool Validate(bool beForgiving)
        {
            throw new NotImplementedException();
        }
    }

    public class Content
    {
        public Alignment Alignment { get; set; } = Alignment.Inherit;
        public Row[] Rows { get; init; }
    }

    public class Header
    {
        public Alignment Alignment { get; set; } = Alignment.Inherit;
        public Row Row { get; init; }
    }

    public class Row
    {
        public Alignment Alignment { get; init; } = Alignment.Inherit;
        public Column[] Columns { get; init; }
    }

    public class Column
    {
        public string Content { get; init; }
        public Alignment Alignment { get; init; } = Alignment.Inherit;
    }

    public class TableOptions
    {
        public int TotalColumns { get; set; }
    }

    public class TableRenderOptions
    {
        public int CellSize { get; }
        public int PaddingLeft { get; }
        public int PaddingRight { get; }
        public int TotalWidthWithoutMargin { get; }
        public int TotalColumns { get; }

        public int MarginLeft { get; } = 1;
        public int MarginRight { get; } = 1;

        public TableRenderOptions(int cellSize, int totalColumns, int paddingLeft = 2, int paddingRight = 2)
        {
            CellSize = cellSize;
            TotalColumns = totalColumns;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            TotalWidthWithoutMargin = cellSize + paddingLeft + paddingRight;
        }
    }

    public enum TableRenderStyle
    {
        Default
    }
}
