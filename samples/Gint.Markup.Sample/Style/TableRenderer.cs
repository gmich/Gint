using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gint.Markup.Sample
{
    public interface ITableRenderVisitor
    {
        void PreWrite(string text, TableSection section);
        void PostWrite(string text,  TableSection section);
    }

    public class TestTableRenderVisitor : ITableRenderVisitor
    {
        public void PostWrite(string text,  TableSection section)
        {
            Console.ResetColor();
        }

        public void PreWrite(string text,  TableSection section)
        {
            if (section == TableSection.HeaderColumn)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (section == TableSection.ContentColumn)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    public class TableRenderer
    {
        private readonly TableRenderPreferences tablePreferences;
        private readonly ITableRenderVisitor tableRenderVisitor;
        private readonly TableBorder border;
        private readonly TableInside inside;
        private readonly TableRenderParameters renderOptions;
        private List<int> columnSegmentSizes = new List<int>();

        public Table Table { get; }
        public TextWriter Writer { get; }

        public TableRenderer(Table table, TextWriter writer, TableRenderPreferences tablePreferences, ITableRenderVisitor tableRenderVisitor =null)
        {
            this.tablePreferences = tablePreferences;
            this.tableRenderVisitor = tableRenderVisitor ?? new TestTableRenderVisitor();
            Table = table;
            Writer = writer;
            inside = tablePreferences.TableStyle.TableInsight;
            border = tablePreferences.TableStyle.TableBorder;
            renderOptions = new TableRenderParameters(table, tablePreferences);
        }

        public void Render()
        {
            RenderBorderTop();

            ChangeLine();

            RenderBorderLeft();
            RenderHeaderColumns();
            RenderBorderRight();

            ChangeLine();

            RenderBorderLeft();
            RenderHeaderRowDivider();
            RenderBorderRight();

            ChangeLine();

            for (int rowIndex = 0; rowIndex < Table.Content.Rows.Length; rowIndex++)
            {
                RenderBorderLeft();
                RenderContentColumn(rowIndex);
                RenderBorderRight();

                ChangeLine();

                //render all except bottom border
                if (rowIndex < Table.Content.Rows.Length - 1)
                {
                    RenderBorderLeft();
                    RenderContentRowDivider();
                    RenderBorderRight();
                    ChangeLine();
                }
            }
            RenderBorderBottom();

            ChangeLine();
        }

        private void RenderContentColumn(int j)
        {
            Row row = Table.Content.Rows[j];
            for (int i = 0; i < row.Columns.Length; i++)
            {
                Column column = row.Columns[i];
                RenderContentColumn(column);

                if (i < row.Columns.Length - 1)
                    RenderContentColumnDivider();
            }
        }

        private void RenderHeaderColumns()
        {
            for (int i = 0; i < Table.Header.Row.Columns.Length; i++)
            {
                Column column = Table.Header.Row.Columns[i];
                RenderHeaderColumn(column);

                if (i < Table.Header.Row.Columns.Length - 1)
                    RenderHeaderColumnDivider();
            }
        }

        public void ChangeLine()
        {
            Writer.WriteLine();
        }


        private void Write(string text, TableSection section)
        {
            tableRenderVisitor.PreWrite(text, section);
            Writer.Write(text);
            tableRenderVisitor.PostWrite(text, section);
        }


        #region Border

        public void RenderBorderTop()
        {
            Write(border.TopLeft.ToString(), TableSection.BorderTopLeft);
            Write(new string(border.Top, renderOptions.TotalRowCells - 2), TableSection.BorderTop);
            Write(border.TopRight.ToString(), TableSection.BorderTopRight);
        }

        public void RenderBorderLeft()
        {
            Write(border.Left.ToString(), TableSection.BorderLeft);
        }

        public void RenderBorderBottom()
        {
            Write(border.BottomLeft.ToString(), TableSection.BorderBottomLeft);
            Write(new string(border.Bottom, renderOptions.TotalRowCells - 2), TableSection.BorderBottom);
            Write(border.BottomRight.ToString(), TableSection.BorderBottomRight);
        }

        public void RenderBorderRight()
        {
            Write(border.Right.ToString(), TableSection.BorderRight);
        }

        #endregion

        #region Header  

        public void RenderHeaderRowDivider()
        {
            for (int i = 0; i < columnSegmentSizes.Count; i++)
            {
                int segmentSize = columnSegmentSizes[i];
                Write(new string(inside.HeaderRowDivider, segmentSize), TableSection.HeaderRowDivider);

                if (i < columnSegmentSizes.Count - 1)
                    Write(inside.HeaderConnector.ToString(), TableSection.HeaderConnector);
            }
            columnSegmentSizes.Clear();
        }

        public void RenderHeaderColumnDivider()
        {
            Write(inside.HeaderColumnDivider.ToString(), TableSection.HeaderColumnDivider);
        }
        public void RenderHeaderColumn(Column column)
        {
            var rendered = RenderColumn(column, GetAlignment(column, tablePreferences.DefaultHeaderAlignment));
            columnSegmentSizes.Add(rendered.Length);
            Write(rendered, TableSection.HeaderColumn);
        }

        #endregion

        #region Content

        public void RenderContentRowDivider()
        {
            for (int i = 0; i < columnSegmentSizes.Count; i++)
            {
                int segmentSize = columnSegmentSizes[i];
                Write(new string(inside.ContentRowDivider, segmentSize), TableSection.ContentRowDividerSegment);

                if (i < columnSegmentSizes.Count - 1)
                    Write(inside.ContentConnector.ToString(), TableSection.ContentConnector);
            }
            columnSegmentSizes.Clear();
        }

        private void RenderContentColumnDivider()
        {
            Write(inside.ContentColumnDivider.ToString(), TableSection.ContentColumnDivider);
        }

        public void RenderContentColumn(Column column)
        {
            var rendered = RenderColumn(column, GetAlignment(column, tablePreferences.DefaultContentAlignment));
            columnSegmentSizes.Add(rendered.Length);
            Write(rendered, TableSection.ContentColumn);
        }

        #endregion

        #region Columns

        private Alignment GetAlignment(Column column, Alignment @default)
        {
            var alignment = column.Alignment == Alignment.Default ? @default : column.Alignment;

            //fallback to start
            if (alignment == Alignment.Default)
                return Alignment.Start;

            return alignment;
        }

        private string RenderColumn(Column column, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Start:
                    return RenderStart(column);
                case Alignment.Center:
                    return RenderCenter(column);
                case Alignment.End:
                    return RenderEnd(column);
                default:
                    throw new ArgumentException($"Alignment <{alignment}> is not supported.");
            }
        }

        private string RenderStart(Column column)
        {
            var builder = new StringBuilder();

            builder.Append(new string(' ', renderOptions.PaddingLeft));
            builder.Append(column.Content);
            var space = (column.SpansOverColumns * renderOptions.CellSize) - column.Content.Length;
            if (space > 0)
                builder.Append(new string(' ', space));
            builder.Append(new string(' ', renderOptions.PaddingRight * column.SpansOverColumns));

            if (column.SpansOverColumns > 1)
            {
                builder.Append(new string(' ', (column.SpansOverColumns - 1) * renderOptions.ColumnDividerWidth));
                builder.Append(new string(' ', (column.SpansOverColumns - 1) * renderOptions.PaddingLeft));
            }

            return builder.ToString();
        }

        private string RenderEnd(Column column)
        {
            var builder = new StringBuilder();

            var totalWidth = renderOptions.CellSize * column.SpansOverColumns
                + ((column.SpansOverColumns - 1) * renderOptions.ColumnDividerWidth)
                + ((column.SpansOverColumns) * renderOptions.PaddingLeft)
                + ((column.SpansOverColumns - 1) * renderOptions.PaddingRight);

            var offset = totalWidth - column.Content.Length;
            if (offset > 0)
            {
                builder.Append(new string(' ', offset));
            }

            builder.Append(column.Content);
            builder.Append(new string(' ', renderOptions.PaddingRight));

            return builder.ToString();
        }

        private string RenderCenter(Column column)
        {
            var builder = new StringBuilder();

            var totalWidth = renderOptions.CellSize * column.SpansOverColumns
                + ((column.SpansOverColumns - 1) * renderOptions.ColumnDividerWidth)
                + ((column.SpansOverColumns - 1) * renderOptions.PaddingLeft)
                + ((column.SpansOverColumns - 1) * renderOptions.PaddingRight);

            double offset = (totalWidth - column.Content.Length) / 2;

            builder.Append(new string(' ', renderOptions.PaddingLeft + (int)Math.Ceiling(offset)));
            builder.Append(column.Content);
            var space = totalWidth - column.Content.Length - (int)Math.Floor(offset);
            if (space > 0)
                builder.Append(new string(' ', space));
            builder.Append(new string(' ', renderOptions.PaddingRight));

            return builder.ToString();
        }

        #endregion
    }

}
