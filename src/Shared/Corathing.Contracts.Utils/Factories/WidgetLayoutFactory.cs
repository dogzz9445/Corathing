using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Factories;

public static class WidgetLayoutFactory
{
    public static WidgetLayout Create(Guid hostId, WidgetContext context) =>
        new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = hostId,
            Rect = new WidgetLayoutRect()
            {
                X = 0,
                Y = 0,
                W = 2,
                H = 2,
            }
        };

    public static WidgetLayout Create(WidgetContext context) =>
        new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = context.WidgetId,
            Rect = new WidgetLayoutRect()
            {
                X = 0,
                Y = 0,
                W = 2,
                H = 2,
            }
        };

    public static WidgetLayout Create(WidgetState state) =>
        new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = state.Id,
            Rect = new WidgetLayoutRect()
            {
                X = state.CoreSettings.ColumnIndex,
                Y = state.CoreSettings.RowIndex,
                W = state.CoreSettings.ColumnSpan,
                H = state.CoreSettings.RowSpan,
            }
        };
}
