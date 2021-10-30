using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Markup.Sample
{
    internal class RowContext
    {
        internal Alignment Alignment { get; } = Alignment.Default;
        internal bool? SkipRowDivider { get; } = null;
        internal List<Column> Columns { get; } = new List<Column>();
        internal bool IsHeader { get; }

        public RowContext(Alignment alignment, bool? withRowDivider, bool isHeader)
        {
            Alignment = alignment;
            SkipRowDivider = withRowDivider;
            IsHeader = isHeader;
        }
    }
}
