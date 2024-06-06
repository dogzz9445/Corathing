using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Models;
using Corathing.Organizer.Views;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.ViewModels;

/// <summary>
/// View model for dashboards
/// Implements the <see cref="Infrastructure.ViewModelBase" />
/// Implements the <see cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
/// </summary>
/// <seealso cref="Infrastructure.ViewModelBase" />
/// <seealso cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
public partial class DashboardsViewModel : ObservableObject, IDashboardConfigurationHandler
{
    #region Private Fields

    [ObservableProperty]
    private ObservableCollection<ProjectContext>? _projects;

    /// <summary>
    /// Gets or sets the dashboards.
    /// </summary>
    /// <value>The dashboards.</value>
    [ObservableProperty]
    private ObservableCollection<WorkflowContext>? _dashboards;

    [ObservableProperty]
    private WorkflowContext _selectedDashboard;

    [ObservableProperty]
    private ObservableCollection<MenuItemViewModel> _addWidgetMenuItemViewModels;

    /// <summary>
    /// Gets or sets a value indicating whether [dashboard selector uncheck].
    /// </summary>
    /// <value><c>true</c> if [dashboard selector uncheck]; otherwise, <c>false</c>.</value>
    [ObservableProperty]
    private bool? _dashboardSelectorUncheck;

    /// <summary>
    /// Gets or sets a value indicating whether [edit mode].
    /// </summary>
    /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
    [ObservableProperty]
    private bool? _editMode;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    /// Gets the command add widget.
    /// </summary>
    /// <value>The command add widget.</value>
    public ICommand CommandAddWidget => new RelayCommand<WidgetGenerator>(o =>
    {
        var widgetGeneratorToAdd = (WidgetGenerator)o;

        SelectedDashboard.Widgets.Add(widgetGeneratorToAdd.CreateWidget());
        EditMode = true;
    });

    [RelayCommand]
    public void AddWidget(WidgetGenerator generator)
    {
        SelectedDashboard.Widgets.Add(generator.CreateWidget());
    }

    [RelayCommand]
    public void OpenOrganizerSettings()
    {
        var window = new BaseWindow();
        window.Content = new OrganizerSettingsView();
        window.Owner = Application.Current.MainWindow;
        Application.Current.MainWindow.Effect = new BlurEffect();
        window.ShowDialog();
        Application.Current.MainWindow.Effect = null;
    }

    [RelayCommand]
    public void OpenProjectSettings()
    {
        var window = new BaseWindow();
        window.Content = new ProjectSettingsView();
        window.Owner = Application.Current.MainWindow;
        window.ShowDialog();
    }

    [RelayCommand]
    public void ToggleEditDashboard()
    {
        EditMode = !EditMode;
    }

    /// <summary>
    /// Gets the command configure widget.
    /// </summary>
    /// <value>The command configure widget.</value>
    public ICommand CommandConfigureWidget => new RelayCommand<WidgetHost>(async o =>
    {
        var widgetHost = (WidgetHost)o;
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
    });

    /// <summary>
    /// Gets the command done configuring widget.
    /// </summary>
    /// <value>The command done configuring widget.</value>
    //public ICommand CommandDoneConfiguringWidget => new RelayCommand(() => ConfiguringWidget = null);

    /// <summary>
    /// Gets the command edit dashboard.
    /// </summary>
    /// <value>The command edit dashboard.</value>
    //public ICommand CommandEditDashboard => new RelayCommand<string>(o => EditMode = o.ToString() == "True", o => ConfiguringWidget == null);
    public ICommand CommandEditDashboard => new RelayCommand<string>(o => EditMode = o.ToString() == "True", o => true);

    ///// <summary>
    ///// Gets the command manage dashboard.
    ///// </summary>
    ///// <value>The command manage dashboard.</value>
    //public ICommand CommandManageDashboard => new RelayCommand(() =>
    //    ConfiguringDashboard =
    //        new DashboardSettingsPromptViewModel(DashboardConfigurationType.Existing, this,
    //            SelectedDashboard.Title));

    ///// <summary>
    ///// Gets the command new dashboard.
    ///// </summary>
    ///// <value>The command new dashboard.</value>
    //public ICommand CommandNewDashboard => new RelayCommand(() =>
    //    ConfiguringDashboard = new DashboardSettingsPromptViewModel(DashboardConfigurationType.New, this));

    /// <summary>
    /// Gets the command remove widget.
    /// </summary>
    /// <value>The command remove widget.</value>
    public ICommand CommandRemoveWidget => new RelayCommand<WidgetContext>(o => SelectedDashboard.Widgets.Remove((WidgetContext)o));

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Completes dashboard configuration
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="save">if set to <c>true</c> [save].</param>
    /// <param name="newName">The new name.</param>
    public void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName)
    {
        //ConfiguringDashboard = null;

        if (!save)
            return;

        switch (type)
        {
            case DashboardConfigurationType.New:
                var dashboardModel = new WorkflowContext { Title = newName };
                Dashboards.Add(dashboardModel);
                SelectedDashboard = dashboardModel;
                return;
            case DashboardConfigurationType.Existing:
                SelectedDashboard.Title = newName;
                return;
        }
    }

    /// <summary>
    /// Dashboards the name valid.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>DashboardNameValidResponse.</returns>
    public DashboardNameValidResponse DashboardNameValid(string name)
    {
        return Dashboards.Any(dashboard => dashboard.Title == name)
            ? new DashboardNameValidResponse(false, $"That Dashboard Name [{name}] already exists.")
            : new DashboardNameValidResponse(true);
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    /// <returns>Task.</returns>
    public Task Start(IServiceProvider services)
    {
        // --------------------------------------------------------------------------
        // Available Widgets
        // --------------------------------------------------------------------------
        IPackageService widgetService = services.GetService<IPackageService>();

        // --------------------------------------------------------------------------
        // Load Component Data
        // --------------------------------------------------------------------------
        Projects = new ObservableCollection<ProjectContext>
        {
            new ProjectContext { Title = "My Project" }
        };
        Dashboards = [new WorkflowContext { Title = "My Workflow" }];
        SelectedDashboard = Dashboards[0];
        AddWidgetMenuItemViewModels = new ObservableCollection<MenuItemViewModel>();

        // --------------------------------------------------------------------------
        // Add Widget Menu
        // --------------------------------------------------------------------------
        AddWidgetMenuItemViewModels.Add(new MenuItemViewModel()
        {
            Header = "Add Widget",
            MenuItems = new ObservableCollection<MenuItemViewModel>(),
        });

        foreach (var widget in widgetService.GetAvailableWidgets())
        {
            var fullMenuHeader = widget.MenuPath;
            if (string.IsNullOrEmpty(fullMenuHeader))
                continue;

            var splitedMenuHeaders = fullMenuHeader.Split('/');
            if (splitedMenuHeaders.Length <= 0)
                continue;

            var parentMenuCollection = AddWidgetMenuItemViewModels.First().MenuItems;

            for (int i = 0; i < splitedMenuHeaders.Length; i++)
            {
                if (i == splitedMenuHeaders.Length - 1)
                {
                    parentMenuCollection.Add(new MenuItemViewModel()
                    {
                        Header = splitedMenuHeaders[i],
                        Command = new RelayCommand(() =>
                        {
                            SelectedDashboard.Widgets.Add(widget.CreateWidget());
                        }, () => true),
                    });
                }
                else
                {
                    var parentMenu = parentMenuCollection.FirstOrDefault(item => item.Header == splitedMenuHeaders[i]);
                    if (parentMenu == null)
                    {
                        parentMenu = new MenuItemViewModel() { Header = splitedMenuHeaders[i] };
                        parentMenu.MenuItems = new ObservableCollection<MenuItemViewModel>();
                        parentMenuCollection.Add(parentMenu);
                    };
                    parentMenuCollection = parentMenu.MenuItems;
                }
            }

        }

        return Task.CompletedTask;
    }

    #endregion Public Methods
}

