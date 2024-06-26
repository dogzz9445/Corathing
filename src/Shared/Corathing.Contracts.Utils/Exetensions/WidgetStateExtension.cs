using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Exetensions;

public static class WidgetStateExtension
{
    public static void SetWidgetState(this WidgetState widgetState, WidgetState state)
    {
        widgetState.Id = state.Id;
    }

    public static void CopyTo(this WidgetState sourceState, WidgetState targetState)
    {
        targetState.CoreSettings = new WidgetCoreState()
        {
            TypeName = sourceState.CoreSettings.TypeName,
            AssemblyName = sourceState.CoreSettings.AssemblyName,
            RowIndex = sourceState.CoreSettings.RowIndex,
            ColumnIndex = sourceState.CoreSettings.ColumnIndex,
            RowSpan = sourceState.CoreSettings.RowSpan,
            ColumnSpan = sourceState.CoreSettings.ColumnSpan,
            Title = sourceState.CoreSettings.Title,
            Description = sourceState.CoreSettings.Description,
            VisibleTitle = sourceState.CoreSettings.VisibleTitle,
            UseDefaultBackgroundColor = sourceState.CoreSettings.UseDefaultBackgroundColor,
            BackgroundColor = sourceState.CoreSettings.BackgroundColor,
        };
    }
}
