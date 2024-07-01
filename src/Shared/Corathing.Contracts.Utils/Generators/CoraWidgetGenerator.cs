using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Entries;

namespace Corathing.Contracts.Utils.Generators;

public class CoraWidgetGenerator(IServiceProvider services)
{
    public ICoraWidgetInfo Info { get; set; }

    #region Create Methods

    /// <summary>
    /// Creates the widget.
    /// </summary>
    /// <returns>WidgetBase</returns>
    public WidgetContext CreateWidget(WidgetState? state = null)
    {
        if (state == null)
        {
            state = CreateState();
        }
        var context = (WidgetContext)Activator.CreateInstance(Info.WidgetContextType, services, state);
        context.Layout = new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = context.WidgetId,
            Rect = new WidgetLayoutRect()
            {
                X = 0,
                Y = 0,
                W = state.CoreSettings.ColumnSpan,
                H = state.CoreSettings.RowSpan,
            }
        };
        return context;
    }

    /// <summary>
    /// Creates the state.
    /// </summary>
    /// <returns>WidgetState</returns>
    public WidgetState CreateState()
    {
        WidgetState state = new WidgetState();
        state.Id = Guid.NewGuid();
        // TODO:
        // add infos about PackageReference
        state.PackageReference = new PackageReferenceState()
        {
            AssemblyName = Info.WidgetContextType.Assembly.GetType().Name,
        };
        state.CoreSettings = new WidgetCoreState()
        {
            TypeName = Info.WidgetContextType.FullName,

            RowIndex = 0,
            ColumnIndex = 0,
            RowSpan = Info.DefaultRowSpan,
            ColumnSpan = Info.DefaultColumnSpan,

            Title = Info.DefaultTitle,
            Description = Info.Description,
            VisibleTitle = Info.DefaultVisibleTitle,

            UseDefaultBackgroundColor = true,
            BackgroundColor = "#00FF00",

        };
        if (Info.WidgetCustomSettingsType != null)
        {
            state.CustomSettings = Activator.CreateInstance(Info.WidgetCustomSettingsType);
        }
        return state;
    }

    public IWidgetCustomSettingsContext? CreateCustomSettingsContext()
    {
        // FIXME:
        // 현재는 세팅에 대해 수동 생성 방식이지만
        // 추후 자동으로 UI를 변경하는 방식이라면
        // null 체크에 따라 작동 방식이 달라질 수 있다.
        if (Info.WidgetCustomSettingsType == null)
            return null;
        if (Info.WidgetCustomSettingsContextType == null)
            return null;
        var customSettings = Activator.CreateInstance(Info.WidgetCustomSettingsType);
        return (IWidgetCustomSettingsContext)Activator.CreateInstance(Info.WidgetCustomSettingsContextType, customSettings);
    }

    #endregion Create Methods

}
