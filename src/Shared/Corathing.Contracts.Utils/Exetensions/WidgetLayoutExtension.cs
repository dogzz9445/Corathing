using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Exetensions;

public static class WidgetLayoutExtension
{
    public static void CopyTo(this WidgetLayout source, WidgetLayout? target)
    {
        if (target == null)
            return;

        target.WidgetStateId = source.WidgetStateId;
        target.Rect = new WidgetLayoutRect()
        {
            X = source.Rect.X,
            Y = source.Rect.Y,
            W = source.Rect.W,
            H = source.Rect.H,
        };
    }

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
