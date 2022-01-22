using System.Text.Json.Serialization;

namespace Gint.Tables
{
    public class Header
    {
        [JsonInclude]
        public Row[] Rows { get; internal set; }
    }
}
