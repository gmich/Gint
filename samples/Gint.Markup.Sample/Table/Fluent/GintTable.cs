using System.Collections.Generic;

namespace Gint.Markup.Sample
{
    public static class GintTable
    {
        public static TableRenderPreferences TableRenderPreferences { get; set; } = new TableRenderPreferences();

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
