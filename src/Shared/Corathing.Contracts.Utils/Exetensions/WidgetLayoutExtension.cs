using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Exetensions;

public static class WidgetLayoutExtension
{
    public static void UpdateFrom(this WidgetLayout target, WidgetState source)
    {
        target.WidgetStateId = source.Id;
        target.Rect = new WidgetLayoutRect()
        {
            X = source.CoreSettings.ColumnIndex,
            Y = source.CoreSettings.RowIndex,
            W = source.CoreSettings.ColumnSpan,
            H = source.CoreSettings.RowSpan,
        };
    }
}
