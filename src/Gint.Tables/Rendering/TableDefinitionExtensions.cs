using System;
using System.IO;

namespace Gint.Tables
{
    public static class TableDefinitionExtensions
    {
        public static void Render(this TableDefinition definition, TextWriter writer)
        {
            var renderer = new TableRenderer(definition);
            renderer.Render(writer);
        }

        public static void RenderToConsole(this TableDefinition definition)
        {
            definition.Render(Console.Out);
        }
    }

}
