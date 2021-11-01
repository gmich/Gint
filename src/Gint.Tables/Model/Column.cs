using System;
using System.Linq;

namespace Gint.Tables
{
    internal class Column
    {
        internal int SpansOverColumns { get; set; } = 1;
        internal string Content { get; set; } = string.Empty;
        internal Alignment Alignment { get; init; } = Alignment.Default;
        internal string Rendered { get; set; }
        internal Func<string,string> PreRender { get; set; }
        internal Action<string> PostRender { get; set; }
        internal bool SkipRowDivider { get; set; }

        internal int ContentLength => Content.Split(Environment.NewLine).Max(c => c.Length);
    }
}
