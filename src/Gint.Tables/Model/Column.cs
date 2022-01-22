using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Gint.Tables
{
    public class Column
    {
        [JsonInclude]
        public int SpansOverColumns { get; internal  set; } = 1;

        [JsonInclude]
        public string Content { get; internal  set; } = string.Empty;

        [JsonInclude]
        public Alignment Alignment { get; internal set; } = Alignment.Default;

        [JsonInclude]
        public string Rendered { get; internal set; }

        [JsonInclude]
        public bool SkipRowDivider { get; internal set; }

        [JsonInclude]
        public ConsoleColor? ForegroundColor { get; internal set; }

        [JsonIgnore]
        internal int ContentLength => Content.Split(Environment.NewLine).Max(c => c.Length);
    }
}
