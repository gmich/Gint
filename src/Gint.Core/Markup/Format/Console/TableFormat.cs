using Gint.Tables;
using System;

namespace Gint.Markup.Format
{
    internal class TableFormat : AConsoleMarkupTokenFormat
    {
        public override string Tag { get; } = "table";

        public override void Apply(string variable)
        {
            try
            {
                var definition = TableDefinition.FromString(variable);

                definition.Render(Console.Out);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to render table: {Environment.NewLine}{variable}");
            }
        }

    }
}
