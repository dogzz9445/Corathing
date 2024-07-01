using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Dashboards.Controls;

public static class WidgetHostUtils
{
    public static void SetTitle(this IWidgetHost widgetHost, string title)
    {
        widgetHost.Title = title;
    }

    public static void SetEditMode(this IWidgetHost widgetHost, bool editMode)
    {
        widgetHost.EditMode = editMode;
    }
}
