using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gint.Tables
{

    public class TableRenderPreferences
    {

        public TableRenderPreferences()
        {
            TableRenderMiddleware = new List<ITableRenderMiddleware>()
            {
                new SectionColorerMiddleware(this)
            };
        }

        public ConsoleColor HeaderColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor ContentColor { get; set; } = ConsoleColor.DarkCyan;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.DarkMagenta;

        public Alignment DefaultHeaderAlignment { get; set; } = Alignment.Center;
        public Alignment DefaultContentAlignment { get; set; } = Alignment.Center;
        public bool TryFitToScreen { get; set; } = false;
        public int? PreferredTableWidth { get; set; }
        public int ColumnPaddingLeft { get; set; } = 2;
        public int ColumnPaddingRight { get; set; } = 2;
        public TableStyle TableStyle { get; set; } = TableStyle.Square;

        private bool _withHeaderUpperCase = false;
        public bool WithHeaderUpperCase
        {
            get
            {
                return _withHeaderUpperCase;
            }
            set
            {
                if (value)
                    WithHeaderUppercase();
                else
                    WithoutHeaderUppercase();

                _withHeaderUpperCase = value;
            }
        }

        [JsonIgnore]
        public List<ITableRenderMiddleware> TableRenderMiddleware { get; set; }
        public TextOverflow TextOverflow { get; set; } = new TextOverflow();


        public TableRenderPreferences WithColorPallette(ConsoleColor border, ConsoleColor header, ConsoleColor content)
        {
            BorderColor = border;
            HeaderColor = header;
            ContentColor = content;

            return this;
        }

        private void WithHeaderUppercase()
        {
            if (!TableRenderMiddleware.Contains(HeaderUppercaseMiddleware.Instance))
                TableRenderMiddleware.Add(HeaderUppercaseMiddleware.Instance);
        }

        private void WithoutHeaderUppercase()
        {
            if (TableRenderMiddleware.Contains(HeaderUppercaseMiddleware.Instance))
                TableRenderMiddleware.Remove(HeaderUppercaseMiddleware.Instance);
        }
    }
}
