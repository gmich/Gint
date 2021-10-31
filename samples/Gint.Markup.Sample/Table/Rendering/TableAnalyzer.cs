using System;
using System.Collections.Generic;
using System.Linq;

namespace Gint.Markup.Sample
{
    internal class TableAnalyzer
    {
        public static void AdjustTable(Table table, int maxCellSize, TableRenderPreferences preferences)
        {
            if (table.HasHeader)
                table.Header.Rows = AdjustRows(table.Header.Rows, maxCellSize, preferences);

            table.Content.Rows = AdjustRows(table.Content.Rows, maxCellSize, preferences);
        }

        public static Row[] AdjustRows(Row[] rows, int maxCellSize, TableRenderPreferences preferences)
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
                    (bool AddExtraRow, List<Column> ExtraColumns) = AnalyzeRow(rowToAnalyze, maxCellSize, preferences);
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
        private static (bool AddExtraRow, List<Column> ExtraColumns) AnalyzeRow(Row row, int maxCellSize, TableRenderPreferences preferences)
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

                column.Content = HandleTextOverflow(maxCellSize, column.Content, preferences.TextOverflow);

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

        private static string HandleTextOverflow(int maxCellSize, string content, TextOverflow overflow)
        {
            switch (overflow.Overflow)
            {
                case TextOverflowOption.ChangeLine:
                    if (content.Length > maxCellSize)
                    {
                        var newContent = content.Substring(0, maxCellSize);
                        int totalIterationsAllowed = maxCellSize / 2;
                        int totalIterations = 0;
                        for (int i = newContent.Length - 1; i >= 0; i--)
                        {
                            if (totalIterations >= totalIterationsAllowed) break;

                            if (newContent[i] == ' ')
                                return content.Remove(i, 1).Insert(i, Environment.NewLine);

                            totalIterations++;
                        }
                        return content.Insert(maxCellSize, Environment.NewLine);
                    }
                    break;
                case TextOverflowOption.Ellipsis:
                    if (content.Length > maxCellSize)
                    {
                        return content.Remove(maxCellSize - 3) + "...";
                    }
                    break;
                case TextOverflowOption.Clip:
                    if (content.Length > maxCellSize)
                    {
                        return content.Remove(maxCellSize);
                    }
                    break;
                case TextOverflowOption.Render:
                default:
                    break;
            }

            return content;

        }
    }
}
