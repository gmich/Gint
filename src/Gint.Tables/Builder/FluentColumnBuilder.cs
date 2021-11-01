using System;
using System.Collections.Generic;

namespace Gint.Tables
{
    public class FluentColumnBuilder
    {
        private Alignment _alignment;
        private bool? _skipRowDivider = null;
        private int _numberOfColumns = 1;
        private readonly List<RowContext> rows;
        private readonly RowContext _rowBuilder;
        private readonly string _content;
        private ConsoleColor? _consoleColor;

        internal FluentColumnBuilder(List<RowContext> _rows, RowContext rowBuilder, string content)
        {
            rows = _rows;
            _rowBuilder = rowBuilder;
            _content = content ?? string.Empty;
        }

        public FluentColumnBuilder SpansOverColumns(int numberOfColumns)
        {
            _numberOfColumns = numberOfColumns;
            return this;
        }

        public FluentColumnBuilder WithColor(ConsoleColor consoleColor)
        {
            _consoleColor = consoleColor;
            return this;
        }


        public FluentColumnBuilder WithRowDivider()
        {
            _skipRowDivider = false;
            return this;

        }
        public FluentColumnBuilder WithoutRowDivider()
        {
            _skipRowDivider = true;
            return this;

        }

        public FluentColumnBuilder WithAlignment(Alignment alignment)
        {
            _alignment = alignment;
            return this;

        }

        public FluentRowBuilder NewRow()
        {
            InternalAddColumn();
            return new FluentRowBuilder(rows, _rowBuilder);
        }

        public ITableRenderer Build(TableRenderPreferences tableRenderPreferences = null)
        {
            InternalAddColumn();
            return new FluentRowBuilder(rows, _rowBuilder).Build(tableRenderPreferences);
        }

        public FluentColumnBuilder AddColumn(string content)
        {
            InternalAddColumn();
            return new FluentColumnBuilder(rows, _rowBuilder, content);
        }

        private void InternalAddColumn()
        {
            var column = new Column
            {
                Alignment = _alignment,
                Content = _content,
                SkipRowDivider = _skipRowDivider ?? _rowBuilder.SkipRowDivider ?? false,
                SpansOverColumns = _numberOfColumns
            };

            if(_consoleColor.HasValue)
            {
                column.PreRender += str =>
                {
                    Console.ForegroundColor = _consoleColor.Value;
                    return str;
                };
                column.PostRender += str => Console.ResetColor();
            }

            _rowBuilder.Columns.Add(column);
        }
    }
}
