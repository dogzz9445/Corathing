using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Exetensions;

public static class WidgetContextExtension
{
    public static void CopyTo(this WidgetContext sourceContext, WidgetContext? targetContext, Type? customSettingsType = null)
    {
        if (targetContext == null)
            return;

        sourceContext.State?.CopyTo(targetContext.State, customSettingsType);
        targetContext.WidgetId = sourceContext.WidgetId;
        targetContext.WidgetTitle = sourceContext.WidgetTitle;
        targetContext.VisibleTitle = sourceContext.VisibleTitle;
        targetContext.UseDefaultBackgroundColor = sourceContext.UseDefaultBackgroundColor;
        targetContext.BackgroundColor = sourceContext.BackgroundColor;
        sourceContext.Layout?.CopyTo(targetContext.Layout);
    }

    public static void CopyToWithoutLayout(this WidgetContext sourceContext, WidgetContext targetContext, Type? customSettingsType = null)
    {
        if (targetContext == null)
            return;

        sourceContext.State?.CopyTo(targetContext.State, customSettingsType);
        targetContext.WidgetId = sourceContext.WidgetId;
        targetContext.WidgetTitle = sourceContext.WidgetTitle;
        targetContext.VisibleTitle = sourceContext.VisibleTitle;
        targetContext.UseDefaultBackgroundColor = sourceContext.UseDefaultBackgroundColor;
        targetContext.BackgroundColor = sourceContext.BackgroundColor;
    }

    public static void UpdateTo(this WidgetContext sourceContext, WidgetState targetState)
    {
        targetState.PackageReference = sourceContext.State.PackageReference;

        targetState.CoreSettings = new WidgetCoreState()
        {
            TypeName = sourceContext.State.CoreSettings.TypeName,
            RowIndex = sourceContext.Layout.XY.Y,
            ColumnIndex = sourceContext.Layout.XY.X,
            RowSpan = sourceContext.Layout.WH.H,
            ColumnSpan = sourceContext.Layout.WH.W,
            Title = sourceContext.WidgetTitle,
            VisibleTitle = sourceContext.VisibleTitle ?? true,
            UseDefaultBackgroundColor = sourceContext.UseDefaultBackgroundColor ?? true,
            BackgroundColor = sourceContext.BackgroundColor,
        };
    }
}
