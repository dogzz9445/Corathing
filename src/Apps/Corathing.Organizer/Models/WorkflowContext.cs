using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Views;

using Microsoft.Extensions.DependencyInjection;

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
    #region Readonly properties
    private IServiceProvider _services;

    #endregion

    public Guid WorkflowId { get; set; }
    public WorkflowState WorkflowState { get; set; }

    #region Public Properties
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _name = "My Workflow";

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

    public WorkflowContext(IServiceProvider services)
    {
        _services = services;
        Widgets = new ObservableCollection<WidgetContext>();
    }

    public void UpdateWorkflow(WorkflowState workflowState)
    {
        var packageService = _services.GetService<IPackageService>();
        var appStateService = _services.GetService<IAppStateService>();

        Name = workflowState.CoreSettings.Name;
        WorkflowId = workflowState.Id;
        foreach (var widgetStateId in workflowState.WidgetIds)
        {
            if (!appStateService.TryGetWidget(widgetStateId, out var widgetState))
            {
                // TODO:
                // Change Exception Type
                throw new Exception();
            }

            if (!packageService.TryGetWidgetGenerator(widgetState.CoreSettings.TypeName, out var generator))
                continue;

            var widgetContext = generator.CreateWidget(widgetState);
            widgetContext.EditMode = EditMode;
            AddWidget(widgetContext);
        }
    }

    [RelayCommand]
    public void AddWidget(CoraWidgetGenerator generator)
    {
        var context = generator.CreateWidget();
        AddWidget(context);
    }

    public void AddWidget(WidgetContext context)
    {
        var appState = _services.GetService<IAppStateService>();

        if (!appState.TryGetWorkflow(WorkflowId, out var workflowState))
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }

        appState.UpdateWidget(context.State);
        workflowState.WidgetIds.Add(context.WidgetId);
        appState.UpdateWorkflow(workflowState);
        Widgets.Add(context);
    }

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

    [RelayCommand]
    public void LayoutChanged(DashboardHost host)
    {
        var appState = _services.GetService<IAppStateService>();
        if (WorkflowId != null && appState.TryGetWorkflow(WorkflowId, out var workflowState))
        {
            appState.UpdateWorkflow(workflowState);
        }
    }

    protected override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);

        var appState = _services.GetService<IAppStateService>();
        //appState.UpdateWorkflow();
    }


    #endregion Public Properties
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(EditMode))
        { 
            foreach (var widgetContext in Widgets)
            {
                widgetContext.EditMode = EditMode;
            }
        }
    }
}
