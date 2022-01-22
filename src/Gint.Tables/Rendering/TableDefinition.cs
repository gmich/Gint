using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gint.Tables
{
    public class TableDefinition
    {
        [JsonInclude]
        public TableRenderPreferences TableRenderPreferences { get; internal set; }

        [JsonInclude]
        public Table Table { get; internal set; }


        public static TableDefinition FromString(string json) => JsonSerializer.Deserialize<TableDefinition>(json);

        public override string ToString() => JsonSerializer.Serialize(this);
    }

}
