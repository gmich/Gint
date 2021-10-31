using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gint.Markup.Sample
{
    internal class TableRenderer : ITableRenderer
    {
        private readonly TableRenderPreferences tablePreferences;
        private readonly ITableRenderMiddleware tableRenderVisitor;
        private readonly ITableBorderStyle border;
        private readonly ITableDividerStyle divider;
        private readonly IContentConnectorStyle contentConnector;
        private readonly IHeaderConnectorStyle headerConnector;
        private readonly TableRenderContext renderOptions;

        internal Table Table { get; }

        private TextWriter writer;

        public TableRenderer(Table table, TableRenderPreferences tablePreferences)
        {
            this.tablePreferences = tablePreferences;
            tableRenderVisitor = tablePreferences.TableRenderMiddleware;
            Table = table;
            divider = tablePreferences.TableStyle.TablePart;
            border = tablePreferences.TableStyle.TableBorder;
            contentConnector = tablePreferences.TableStyle.ContentConnector;
            headerConnector = tablePreferences.TableStyle.HeaderConnector;
            renderOptions = new TableRenderContext(table, tablePreferences);
        }

        public void Render(TextWriter writer)
        {
            this.writer = writer;
            TableAnalyzer.AdjustTable(Table, renderOptions.CellSize, tablePreferences);
            AnalyzeColumns();

            RenderBorderTop();
            ChangeLine();

            if (Table.HasHeader)
            {
                for (int rowIndex = 0; rowIndex < Table.Header.Rows.Length; rowIndex++)
                {
                    RenderHeaderBorderLeft(column: null);
                    RenderHeaderColumn(rowIndex);
                    RenderHeaderBorderRight(column: null);

                    ChangeLine();

                    if (!Table.Header.Rows[rowIndex].SkipDivider)
                    {
                        RenderHeaderBorderLeft(column: Table.Header.Rows[rowIndex].Columns?.FirstOrDefault());
                        RenderHeaderRowDivider();
                        RenderHeaderBorderRight(column: Table.Header.Rows[rowIndex].Columns?.LastOrDefault());
                        ChangeLine();
                    }
                }
            }

            for (int rowIndex = 0; rowIndex < Table.Content.Rows.Length; rowIndex++)
            {
                RenderContentBorderLeft(column: null);
                RenderContentColumn(rowIndex);
                RenderContentBorderRight(column: null);

                ChangeLine();

                //render all except bottom border
                if (rowIndex < Table.Content.Rows.Length - 1 && !Table.Content.Rows[rowIndex].SkipDivider)
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

        private void AnalyzeColumns()
        {
            void Analyze(Row[] rows, Alignment defaultAlignment)
            {
                foreach (var row in rows)
                {
                    var contentAnalyzedColumns = new List<AnalyzedColumn>();
                    foreach (var column in row.Columns)
                    {
                        for (int i = 0; i < column.SpansOverColumns; i++)
                        {
                            contentAnalyzedColumns.Add(new AnalyzedColumn(
                                column: column,
                                skipColumnDivider: (i + 1) < column.SpansOverColumns,
                                totalCells: renderOptions.TotalWidthWithoutMargin));
                        }
                    }
                    row.AnalyzedColumns = contentAnalyzedColumns;
                }

                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    Row row = rows[rowIndex];
                    for (int i = 0; i < row.Columns.Length; i++)
                    {
                        Column column = row.Columns[i];
                        var rendered = RenderColumn(column, GetAlignment(column, row.Alignment, defaultAlignment));
                        column.Rendered = rendered;
                    }
                }
            }
            if (Table.Header != null)
                Analyze(Table.Header.Rows, tablePreferences.DefaultHeaderAlignment);

            Analyze(Table.Content.Rows, tablePreferences.DefaultContentAlignment);
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

        private void RenderHeaderColumn(int j)
        {
            Row row = Table.Header.Rows[j];
            for (int i = 0; i < row.Columns.Length; i++)
            {
                Column column = row.Columns[i];
                RenderHeaderColumn(column);

                if (i < row.Columns.Length - 1)
                    RenderHeaderColumnDivider();
            }
        }

        private void ChangeLine()
        {
            writer.WriteLine();
        }

        private void Write(char ch, TableSection section)
        {
            Write(ch.ToString(), section);
        }

        private void Write(string text, TableSection section)
        {
            var newText = tableRenderVisitor.PreWrite(text, section);
            writer.Write(newText);
            tableRenderVisitor.PostWrite(newText, section);
        }

        #region Border

        private void RenderBorderTop()
        {
            var analyzedColumns = Table.Header?.Rows.First().AnalyzedColumns ?? Table.Content.Rows.First().AnalyzedColumns;
            Write(border.Get(TableBorderPart.TopLeft), TableSection.BorderTop);
            for (int i = 0; i < analyzedColumns.Count; i++)
            {
                AnalyzedColumn column = analyzedColumns[i];
                Write(new string(border.Get(TableBorderPart.Top), column.TotalCells), TableSection.BorderTop);
                if (column.SkipColumnDivider)
                    Write(border.Get(TableBorderPart.Top), TableSection.BorderTop);
                else if ((i + 1) == analyzedColumns.Count)
                    break;
                else
                    Write(headerConnector.Get(TableConnectorPart.Top), TableSection.BorderTop);
            }
            Write(border.Get(TableBorderPart.TopRight), TableSection.BorderTop);
        }

        private void RenderHeaderBorderLeft(Column column)
        {
            RenderBorderLeft(column, headerConnector);
        }
        private void RenderContentBorderLeft(Column column)
        {
            RenderBorderLeft(column, contentConnector);
        }

        private void RenderBorderLeft(Column column, IConnectorStyle style)
        {
            if (column == null)
                Write(border.Get(TableBorderPart.Left), TableSection.BorderLeft);
            else if (column.SkipRowDivider)
                Write(border.Get(TableBorderPart.Left), TableSection.BorderLeft);
            else
                Write(contentConnector.Get(TableConnectorPart.Left), TableSection.BorderLeft);
        }

        private void RenderHeaderBorderRight(Column column)
        {
            RenderBorderRight(column, headerConnector);
        }

        private void RenderContentBorderRight(Column column)
        {
            RenderBorderRight(column, contentConnector);
        }

        private void RenderBorderRight(Column column, IConnectorStyle style)
        {
            if (column == null)
                Write(border.Get(TableBorderPart.Right), TableSection.BorderRight);
            else if (column.SkipRowDivider)
                Write(border.Get(TableBorderPart.Right), TableSection.BorderRight);
            else
                Write(style.Get(TableConnectorPart.Right), TableSection.BorderRight);
        }

        private void RenderBorderBottom()
        {
            var flattenedColumns = Table.Content.Rows.Last().AnalyzedColumns;

            Write(border.Get(TableBorderPart.BottomLeft), TableSection.BorderBottom);
            for (int i = 0; i < flattenedColumns.Count; i++)
            {
                AnalyzedColumn column = flattenedColumns[i];
                Write(new string(border.Get(TableBorderPart.Bottom), column.TotalCells), TableSection.BorderBottom);
                if (column.SkipColumnDivider)
                    Write(border.Get(TableBorderPart.Bottom), TableSection.BorderBottom);
                else if ((i + 1) == flattenedColumns.Count)
                    break;
                else
                    Write(contentConnector.Get(TableConnectorPart.Bottom), TableSection.BorderBottom);
            }
            Write(border.Get(TableBorderPart.BottomRight), TableSection.BorderBottom);

        }


        #endregion

        #region Header  

        private void RenderRowDivider(Row currentRow, Column[] nextColumns, char rowDivider, TableSection section, IConnectorStyle style)
        {
            var nextColumnConnections = new List<int>();
            int previous = 0;
            foreach (var column in nextColumns)
            {
                nextColumnConnections.Add(previous + column.Rendered.Length);
                previous += (column.Rendered.Length + renderOptions.ColumnDividerWidth);
            }

            int previousConnectionCell = 0;
            var flattenedColumns = currentRow.AnalyzedColumns;

            var columnsLength = flattenedColumns.Count;

            void Connect(TableConnectorPart c)
            {
                Write(style.Get(c), section);
            }

            for (int i = 0; i < flattenedColumns.Count; i++)
            {
                var flattenedColumn = flattenedColumns[i];
                int segmentSize = flattenedColumn.TotalCells;
                int currentCell = previousConnectionCell + segmentSize;

                Write(new string(flattenedColumn.Column.SkipRowDivider ? ' ' : rowDivider, segmentSize), section);

                if (i + 1 == columnsLength) break;

                if (flattenedColumns[i + 1].SkipRowDivider && flattenedColumns[i].SkipRowDivider)
                    Connect(TableConnectorPart.Blank);
                else if (flattenedColumns[i + 1].SkipRowDivider)
                    if (nextColumnConnections.Contains(currentCell))
                        Connect(TableConnectorPart.Right);
                    else
                        Connect(TableConnectorPart.BottomRight);
                else if (flattenedColumns[i].SkipRowDivider)
                    if (nextColumnConnections.Contains(currentCell))
                        Connect(TableConnectorPart.Left);
                    else
                        Connect(TableConnectorPart.BottomLeft);
                else if (flattenedColumn.SkipColumnDivider)
                    if (nextColumnConnections.Contains(currentCell))
                        Connect(TableConnectorPart.Top);
                    else
                        Connect(TableConnectorPart.Straight);
                else if (nextColumnConnections.Contains(currentCell))
                    Connect(TableConnectorPart.Cross);
                else
                    Connect(TableConnectorPart.Bottom);
                previousConnectionCell += (segmentSize + renderOptions.ColumnDividerWidth);
            }
        }

        private void RenderHeaderRowDivider()
        {
            RenderRowDivider(
                currentRow: Table.Header.Rows.Last(),
                nextColumns: Table.Content.Rows.Length > 0 ? Table.Content.Rows[0].Columns : new Column[0],
                rowDivider: divider.Get(TableDividerPart.HeaderRow),
                section: TableSection.HeaderRowDivider,
                style: headerConnector
                );
        }

        private void RenderHeaderColumnDivider()
        {
            Write(divider.Get(TableDividerPart.HeaderColumn), TableSection.HeaderColumnDivider);
        }

        private void RenderHeaderColumn(Column column)
        {
            var rendered = column.Rendered;
            Write(rendered, TableSection.HeaderColumn);
        }

        #endregion

        #region Content

        private void RenderContentRowDivider(int currentColumn)
        {
            RenderRowDivider(
                currentRow: Table.Content.Rows[currentColumn],
                nextColumns: Table.Content.Rows.Length > currentColumn + 1 ? Table.Content.Rows[currentColumn + 1].Columns : new Column[0],
                rowDivider: divider.Get(TableDividerPart.ContentRow),
                section: TableSection.ContentRowDivider,
                style: contentConnector);
        }

        private void RenderContentColumnDivider()
        {
            Write(divider.Get(TableDividerPart.ContentColumn), TableSection.ContentColumnDivider);
        }

        private void RenderContentColumn(Column column)
        {
            var rendered = column.Rendered;
            Write(rendered, TableSection.ContentColumn);
        }

        #endregion

        #region Columns

        private Alignment GetAlignment(Column column, Alignment rowDefault, Alignment globalDefault)
        {
            var alignment = column.Alignment == Alignment.Default ? rowDefault : column.Alignment;

            if (alignment == Alignment.Default)
                alignment = globalDefault;

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
