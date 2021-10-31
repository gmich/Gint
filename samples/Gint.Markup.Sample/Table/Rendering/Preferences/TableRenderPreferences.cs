using System;
using System.Collections.Generic;

namespace Gint.Markup.Sample
{
    public class TableRenderPreferences
    {
        private SectionColorerMiddleware SectionColorerMiddleware { get; }

        public TableRenderPreferences()
        {
            SectionColorerMiddleware = new SectionColorerMiddleware();
            TableRenderMiddleware = new List<ITableRenderMiddleware>()
            {
                SectionColorerMiddleware
            };
        }

        public Alignment DefaultHeaderAlignment { get; set; } = Alignment.Center;
        public Alignment DefaultContentAlignment { get; set; } = Alignment.Center;
        public bool TryFitToScreen { get; set; } = true;
        public int ColumnPaddingLeft { get; set; } = 2;
        public int ColumnPaddingRight { get; set; } = 2;
        public TableStyle TableStyle { get; set; } = SquareTable.Style;
        public List<ITableRenderMiddleware> TableRenderMiddleware { get; set; }
        public TextOverflow TextOverflow { get; set; } = new TextOverflow();


        public TableRenderPreferences WithColorPallette(ConsoleColor border, ConsoleColor header, ConsoleColor content)
        {
            SectionColorerMiddleware.Border = border;
            SectionColorerMiddleware.Header = header;
            SectionColorerMiddleware.Content = content;

            return this;
        }

        public TableRenderPreferences WithHeaderUppercase()
        {
            if (!TableRenderMiddleware.Contains(HeaderUppercaseMiddleware.Instance))
                TableRenderMiddleware.Add(HeaderUppercaseMiddleware.Instance);
            
            return this;
        }

        public TableRenderPreferences WithoutHeaderUppercase()
        {
            if (TableRenderMiddleware.Contains(HeaderUppercaseMiddleware.Instance))
                TableRenderMiddleware.Remove(HeaderUppercaseMiddleware.Instance);

            return this;
        }
    }
}
