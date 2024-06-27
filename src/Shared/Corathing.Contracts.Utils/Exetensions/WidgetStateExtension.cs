using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Exetensions;

public static class WidgetStateExtension
{
    public static void SetWidgetState(this WidgetState widgetState, WidgetState? state)
    {
        if (state == null)
            return;

        widgetState.Id = state.Id;
    }

    public static void CopyTo(this WidgetState sourceState, WidgetState? targetState, Type? customSettingsType = null)
    {
        if (targetState == null)
            return;

        if (targetState.CoreSettings == null)
            targetState.CoreSettings = new WidgetCoreState();

        targetState.CoreSettings.TypeName = sourceState.CoreSettings.TypeName;
        targetState.CoreSettings.AssemblyName = sourceState.CoreSettings.AssemblyName;
        targetState.CoreSettings.RowIndex = sourceState.CoreSettings.RowIndex;
        targetState.CoreSettings.ColumnIndex = sourceState.CoreSettings.ColumnIndex;
        targetState.CoreSettings.RowSpan = sourceState.CoreSettings.RowSpan;
        targetState.CoreSettings.ColumnSpan = sourceState.CoreSettings.ColumnSpan;
        targetState.CoreSettings.Title = sourceState.CoreSettings.Title;
        targetState.CoreSettings.Description = sourceState.CoreSettings.Description;
        targetState.CoreSettings.VisibleTitle = sourceState.CoreSettings.VisibleTitle;
        targetState.CoreSettings.UseDefaultBackgroundColor = sourceState.CoreSettings.UseDefaultBackgroundColor;
        targetState.CoreSettings.BackgroundColor = sourceState.CoreSettings.BackgroundColor;


        if (sourceState.CustomSettings != null && customSettingsType != null)
        {
            CopyProperties(sourceState.CustomSettings, targetState.CustomSettings, customSettingsType);
        }
    }

    public static void CopyProperties(object? source, object? destination, Type type)
    {
        if (source == null || destination == null)
            throw new ArgumentNullException("Source or/and Destination objects are null");

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            if (property.CanRead && property.CanWrite)
            {
                object value = property.GetValue(source);
                property.SetValue(destination, value);
            }
        }
    }
}
