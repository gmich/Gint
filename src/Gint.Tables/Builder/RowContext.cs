using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gint.Tables
{
    internal class RowContext
    {
        internal bool IsEmpty { get; set; }
        internal Alignment Alignment { get; } = Alignment.Default;
        internal bool? SkipRowDivider { get; } = null;
        internal List<Column> Columns { get; } = new List<Column>();
        internal bool IsHeader { get; }

        public RowContext(Alignment alignment, bool? skipRowDivider, bool isHeader)
        {
            Alignment = alignment;
            SkipRowDivider = skipRowDivider;
            IsHeader = isHeader;
        }
    }
}
