using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gint.Markup.Sample
{
    public interface ITableRenderVisitor
    {
        void PreWrite(string text, TableSection section);
        void PostWrite(string text, TableSection section);
    }

    public class TestTableRenderVisitor : ITableRenderVisitor
    {
        public void PostWrite(string text, TableSection section)
        {
            Console.ResetColor();
        }

        public void PreWrite(string text, TableSection section)
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
                //Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    public class TableRenderer
    {
        private readonly TableRenderPreferences tablePreferences;
        private readonly ITableRenderVisitor tableRenderVisitor;
        private readonly TableBorder border;
        private readonly TablePart inside;
        private readonly TableConnectorPart connector;
        private readonly TableRenderParameters renderOptions;

        public Table Table { get; }
        public TextWriter Writer { get; }

        public TableRenderer(Table table, TextWriter writer, TableRenderPreferences tablePreferences, ITableRenderVisitor tableRenderVisitor = null)
        {
            this.tablePreferences = tablePreferences;
            this.tableRenderVisitor = tableRenderVisitor ?? new TestTableRenderVisitor();
            Table = table;
            Writer = writer;
            inside = tablePreferences.TableStyle.TablePart;
            border = tablePreferences.TableStyle.TableBorder;
            connector = tablePreferences.TableStyle.Connector;
            renderOptions = new TableRenderParameters(table, tablePreferences);
        }

        public void Render()
        {
            PreRenderColumns();

            RenderBorderTop();

            ChangeLine();

            RenderHeaderBorderLeft(column: null);
            RenderHeaderColumns();
            RenderHeaderBorderRight(column: null);

            ChangeLine();

            RenderHeaderBorderLeft(column: Table.Header.Row.Columns?.FirstOrDefault());
            RenderHeaderRowDivider();
            RenderHeaderBorderRight(column: Table.Header.Row.Columns?.LastOrDefault());

            ChangeLine();

            for (int rowIndex = 0; rowIndex < Table.Content.Rows.Length; rowIndex++)
            {
                RenderContentBorderLeft(column: null);
                RenderContentColumn(rowIndex);
                RenderContentBorderRight(column: null);

                ChangeLine();

                //render all except bottom border
                if (rowIndex < Table.Content.Rows.Length - 1)
                {
                    RenderContentBorderLeft(column: Table.Content.Rows[rowIndex].Columns.FirstOrDefault());
                    RenderContentRowDivider(rowIndex);
                    RenderContentBorderRight(column: Table.Content.Rows[rowIndex].Columns.LastOrDefault());
                    ChangeLine();
                }
            }
            RenderBorderBottom();

            ChangeLine();
        }

        private void PreRenderColumns()
        {
            for (int i = 0; i < Table.Header.Row.Columns.Length; i++)
            {
                Column column = Table.Header.Row.Columns[i];
                var rendered = RenderColumn(column, GetAlignment(column, tablePreferences.DefaultHeaderAlignment));
                column.Rendered = rendered;
            }

            for (int rowIndex = 0; rowIndex < Table.Content.Rows.Length; rowIndex++)
            {
                Row row = Table.Content.Rows[rowIndex];
                for (int i = 0; i < row.Columns.Length; i++)
                {
                    Column column = row.Columns[i];
                    var rendered = RenderColumn(column, GetAlignment(column, tablePreferences.DefaultContentAlignment));
                    column.Rendered = rendered;
                }
            }
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

        private void Write(TableConnector con, Func<TableConnector, char> tablePartGetter)
        {
            var text = tablePartGetter(con).ToString();
            Writer.Write(text);
        }


        #region Border

        public void RenderBorderTop()
        {
            var flattenedColumns = new List<FlattenedColumn>();
            foreach (var column in Table.Header.Row.Columns)
            {
                for (int i = 0; i < column.SpansOverColumns; i++)
                {
                    flattenedColumns.Add(new FlattenedColumn(
                        column: column,
                        skipColumnDivider: (i + 1) < column.SpansOverColumns,
                        totalCells: renderOptions.TotalWidthWithoutMargin));
                }
            }

            Write(border.TopLeft.ToString(), TableSection.BorderTopLeft);
            for (int i = 0; i < flattenedColumns.Count; i++)
            {
                FlattenedColumn column = flattenedColumns[i];
                Write(new string(border.Top, column.TotalCells), TableSection.BorderTop);
                if(column.SkipColumnDivider)
                    Write(border.Top.ToString(), TableSection.BorderTop);
                else if ((i + 1) == flattenedColumns.Count)
                    break;
                else
                    Write(TableConnector.Top, c=> connector.GetHeaderConnector(c));
            }
            Write(border.TopRight.ToString(), TableSection.BorderTopRight);
        }

        public void RenderHeaderBorderLeft(Column column)
        {
            if (column == null)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else if (column.SkipRowDivider)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else
                Write(TableConnector.Left, c => connector.GetHeaderConnector(c));
        }

        public void RenderContentBorderLeft(Column column)
        {
            if (column == null)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else if (column.SkipRowDivider)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else
                Write(TableConnector.Left, c => connector.GetContentConnector(c));
        }

        public void RenderHeaderBorderRight(Column column)
        {
            if (column == null)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else if (column.SkipRowDivider)
                Write(border.Left.ToString(), TableSection.BorderLeft);
            else
                Write(TableConnector.Right, c => connector.GetHeaderConnector(c));
        }

        public void RenderContentBorderRight(Column column)
        {
            if (column == null)
                Write(border.Right.ToString(), TableSection.BorderRight);
            else if (column.SkipRowDivider)
                Write(border.Right.ToString(), TableSection.BorderRight);
            else
                Write(TableConnector.Right, c => connector.GetContentConnector(c));
        }


        public void RenderBorderBottom()
        {
            var flattenedColumns = new List<FlattenedColumn>();
            foreach (var column in Table.Content.Rows.Last().Columns)
            {
                for (int i = 0; i < column.SpansOverColumns; i++)
                {
                    flattenedColumns.Add(new FlattenedColumn(
                        column: column,
                        skipColumnDivider: (i + 1) < column.SpansOverColumns,
                        totalCells: renderOptions.TotalWidthWithoutMargin));
                }
            }

            Write(border.BottomLeft.ToString(), TableSection.BorderBottomLeft);
            for (int i = 0; i < flattenedColumns.Count; i++)
            {
                FlattenedColumn column = flattenedColumns[i];
                Write(new string(border.Top, column.TotalCells), TableSection.BorderTop);
                if (column.SkipColumnDivider)
                    Write(border.Bottom.ToString(), TableSection.BorderTop);
                else if ((i + 1) == flattenedColumns.Count)
                    break;
                else
                    Write(TableConnector.Bottom, c => connector.GetContentConnector(c));
            }
            Write(border.BottomRight.ToString(), TableSection.BorderBottomRight);

        }


        #endregion

        #region Header  

        private struct FlattenedColumn
        {
            public FlattenedColumn(Column column, bool skipColumnDivider, int totalCells)
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

        public void RenderRowDivider(Column[] currentColumns, Column[] nextColumns, char rowDivider, TableSection divider, Func<TableConnector, char> tablePartGetter)
        {
            var nextColumnConnections = new List<int>();
            int previous = 0;
            foreach (var column in nextColumns)
            {
                nextColumnConnections.Add(previous + column.Rendered.Length);
                previous += (column.Rendered.Length + renderOptions.ColumnDividerWidth);
            }

            int previousConnectionCell = 0;
            var flattenedColumns = new List<FlattenedColumn>();
            foreach (var column in currentColumns)
            {
                for (int i = 0; i < column.SpansOverColumns; i++)
                {
                    flattenedColumns.Add(new FlattenedColumn(
                        column: column,
                        skipColumnDivider: (i + 1) < column.SpansOverColumns,
                        totalCells: renderOptions.TotalWidthWithoutMargin));
                }
            }

            var columnsLength = flattenedColumns.Count;

            for (int i = 0; i < flattenedColumns.Count; i++)
            {
                var flattenedColumn = flattenedColumns[i];
                int segmentSize = flattenedColumn.TotalCells;
                Write(new string(flattenedColumn.Column.SkipRowDivider ? ' ' : rowDivider, segmentSize), divider);

                if (i + 1 == columnsLength) break;

                if (flattenedColumns[i + 1].SkipRowDivider)
                    if (nextColumnConnections.Contains(previousConnectionCell + segmentSize))
                        Write(TableConnector.Right, tablePartGetter);
                    else
                        Write(TableConnector.BottomRight, tablePartGetter);
                else if (flattenedColumns[i].SkipRowDivider)
                    if (nextColumnConnections.Contains(previousConnectionCell + segmentSize))
                        Write(TableConnector.Left, tablePartGetter);
                    else
                        Write(TableConnector.BottomLeft, tablePartGetter);
                else if (flattenedColumn.SkipColumnDivider)
                    if (nextColumnConnections.Contains(previousConnectionCell + segmentSize))
                        Write(TableConnector.Top, tablePartGetter);
                    else
                        Write(TableConnector.Straight, tablePartGetter);
                else if (nextColumnConnections.Contains(previousConnectionCell + segmentSize))
                    Write(TableConnector.Cross, tablePartGetter);
                else
                    Write(TableConnector.Bottom, tablePartGetter);

                previousConnectionCell += (segmentSize + renderOptions.ColumnDividerWidth);
            }
        }

        public void RenderHeaderRowDivider()
        {
            RenderRowDivider(
                currentColumns: Table.Header.Row.Columns,
                nextColumns: Table.Content.Rows.Length > 0 ? Table.Content.Rows[0].Columns : new Column[0],
                rowDivider: inside.HeaderRowDivider,
                divider: TableSection.HeaderRowDivider,
                tablePartGetter: ch => connector.GetHeaderConnector(ch)
                );
        }

        public void RenderHeaderColumnDivider()
        {
            Write(inside.HeaderColumnDivider.ToString(), TableSection.HeaderColumnDivider);
        }

        public void RenderHeaderColumn(Column column)
        {
            var rendered = column.Rendered;
            Write(rendered, TableSection.HeaderColumn);
        }

        #endregion

        #region Content

        public void RenderContentRowDivider(int currentColumn)
        {
            RenderRowDivider(
                currentColumns: Table.Content.Rows[currentColumn].Columns,
                nextColumns: Table.Content.Rows.Length > currentColumn + 1 ? Table.Content.Rows[currentColumn + 1].Columns : new Column[0],
                rowDivider: inside.ContentRowDivider,
                divider: TableSection.ContentRowDivider,
                tablePartGetter: ch => connector.GetContentConnector(ch)
                );
        }

        private void RenderContentColumnDivider()
        {
            Write(inside.ContentColumnDivider.ToString(), TableSection.ContentColumnDivider);
        }

        public void RenderContentColumn(Column column)
        {
            var rendered = column.Rendered;
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
