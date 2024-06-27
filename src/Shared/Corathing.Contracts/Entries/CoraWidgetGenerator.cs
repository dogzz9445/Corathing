using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Entries;

public class CoraWidgetGenerator
{
    #region Private Fields

    public IServiceProvider Services { get; set; }

    #endregion Private Fields

    #region Public Properties

    public ICoraWidgetInfo Info { get; set; }

    public Type ViewType { get; }
    public Type ContextType { get; }
    public Type CustomSettingsType { get; }
    public Type CustomSettingsContextType { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Widget"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="createWidget">The create widget.</param>
    public CoraWidgetGenerator(
        Type viewType,
        Type contextType,
        Type customSettingsType,
        Type customSettingsContextType
        )
    {
        ViewType = viewType;
        ContextType = contextType;
        CustomSettingsType = customSettingsType;
        CustomSettingsContextType = customSettingsContextType;
    }

    #endregion Public Constructors

    #region Public Methods

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
        var context = (WidgetContext)Activator.CreateInstance(ContextType, Services, state);
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
        state.CoreSettings = new WidgetCoreState()
        {
            TypeName = ContextType.FullName,
            AssemblyName = ContextType.Assembly.FullName,

            RowIndex = 0,
            ColumnIndex = 0,
            RowSpan = Info.DefaultRowSpan,
            ColumnSpan = Info.DefaultColumnSpan,

            Title = Info.Title,
            Description = Info.Description,
            VisibleTitle = Info.VisibleTitle,

            UseDefaultBackgroundColor = true,
            BackgroundColor = "#00FF00",

        };
        if (CustomSettingsType != null)
        {
            state.CustomSettings = Activator.CreateInstance(CustomSettingsType);
        }
        return state;
    }

    public IWidgetCustomSettingsContext? CreateCustomSettingsContext()
    {
        // FIXME:
        // 현재는 세팅에 대해 수동 생성 방식이지만
        // 추후 자동으로 UI를 변경하는 방식이라면
        // null 체크에 따라 작동 방식이 달라질 수 있다.
        if (CustomSettingsType == null)
            return null;
        if (CustomSettingsContextType == null)
            return null;
        var customSettings = Activator.CreateInstance(CustomSettingsType);
        return (IWidgetCustomSettingsContext)Activator.CreateInstance(CustomSettingsContextType, customSettings);
    }

    #endregion Public Methods
}
