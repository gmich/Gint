using System.Collections.Generic;

namespace Gint.Tables
{
    public static class GintTable
    {
        public static TableRenderPreferences DefaultTableRenderPreferences { get; set; } = new TableRenderPreferences();

        public static FluentRowBuilder WithFirstRowAsHeader()
        {
            return new FluentRowBuilder(new List<RowContext>(), newRow: null, isHeader: true);
        }

        public static FluentRowBuilder WithoutHeader()
        {
            return new FluentRowBuilder(new List<RowContext>(), newRow: null, isHeader: false);
        }
    }
}
