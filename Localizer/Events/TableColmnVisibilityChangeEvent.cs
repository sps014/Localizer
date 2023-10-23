using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Events
{
    internal class TableColmnVisibilityChangeEvent
    {
        public TableColumnInfo ColumnInfo { get;init; }

        public TableColmnVisibilityChangeEvent(TableColumnInfo columnInfo)
        {
            this.ColumnInfo = columnInfo;
        }

    }
}
