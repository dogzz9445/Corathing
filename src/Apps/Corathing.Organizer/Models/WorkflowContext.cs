using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Views;

namespace Corathing.Organizer.Models;

/// <summary>
/// Represents the type of configuration for a Dashboard. New meaning a new one is being generated, and
/// existing when the dashboard already exists
/// </summary>
public enum DashboardConfigurationType
{
    /// <summary>
    /// New dashboard being generated
    /// </summary>
    New,

    /// <summary>
    /// Existing dashboard being configured
    /// </summary>
    Existing
}
/// <summary>
/// Represents a dashboard model containing widgets and a title
/// Implements the <see cref="Infrastructure.ViewModelBase" />
/// </summary>
/// <seealso cref="Infrastructure.ViewModelBase" />
public partial class WorkflowContext : ObservableObject
{
    #region Public Properties
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _title;

    [DefaultValue(false)]
    [ObservableProperty]
    private bool? _isPlaceholder;

    [DefaultValue(false)]
    [ObservableProperty]
    private bool? _editMode;

    /// <summary>
    /// Gets or sets the widgets.
    /// </summary>
    /// <value>The widgets.</value>
    [ObservableProperty]
    private ObservableCollection<WidgetContext> _widgets;

    //private IAppStateService? _appState;

    //[ObservableProperty]
    //private WorkflowState? _workflow;
    //[ObservableProperty]
    //private ObservableCollection<WidgetHost> _widgetHosts;
    //[ObservableProperty]
    //private ObservableCollection<WidgetLayouts> _widgetLayouts;

    //public DashboardHostViewModel()
    //{
    //    WidgetHosts = new ObservableCollection<WidgetHost>();
    //    WidgetLayouts = new ObservableCollection<WidgetLayouts>();
    //}

    //public Task Start(IServiceProvider services, Guid workflowId)
    //{
    //    _appState = services.GetService<IAppStateService>();
    //    if (_appState != null && _appState.TryGetWorkflow(workflowId, out var workflow))
    //    {
    //        Workflow = workflow;
    //    }
    //    else
    //    {
    //        Workflow = null;
    //    }
    //    return Task.CompletedTask;
    //}

    /// <summary>
    /// Gets the command remove widget.
    /// </summary>
    /// <value>The command remove widget.</value>
    [RelayCommand]
    public void RemoveWidget(WidgetHost widget)
    {
        Widgets.Remove(widget.DataContext as WidgetContext);
    }

    [RelayCommand]
    public void ConfigureWidget(WidgetHost widget)
    {
        var widgetHost = widget;
        var parentWindow = Window.GetWindow(widgetHost);
        var window = new BaseWindow();
        if (parentWindow != null)
        {
            window.Owner = parentWindow;
            parentWindow.Effect = new BlurEffect();
            window.CenterWindowToParent();
        }
        var view = new WidgetSettingsView(widgetHost);
        window.Content = view;
        window.ShowDialog();
        if (parentWindow != null)
        {
            parentWindow.Effect = null;
        }
    }

    public WorkflowContext()
    {
        Widgets = new ObservableCollection<WidgetContext>();
    }

    #endregion Public Properties
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Widgets != null)
        {
            foreach (var widgetContext in Widgets)
            {
                widgetContext.EditMode = EditMode;
            }
        }
    }
}
