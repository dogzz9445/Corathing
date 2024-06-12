using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;

using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Models;
using Corathing.Organizer.Views;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Corathing.Organizer.ViewModels;

/// <summary>
/// View model for dashboards
/// Implements the <see cref="Infrastructure.ViewModelBase" />
/// Implements the <see cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
/// </summary>
/// <seealso cref="Infrastructure.ViewModelBase" />
/// <seealso cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
public partial class DashboardViewModel : ObservableObject
{
    #region Private Fields

    [ObservableProperty]
    private ObservableCollection<ProjectContext>? _projects;

    [ObservableProperty]
    private ProjectContext _selectedProject;

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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Projects != null)
        {
            foreach (var project in Projects)
            {
                project.EditMode = EditMode;
            }
        }
    }

    [RelayCommand]
    public void AddWidget(CoraWidgetGenerator generator)
    {
        if (SelectedProject == null)
            return;
        if (SelectedProject.SelectedWorkflow == null)
            return;
        SelectedProject.SelectedWorkflow.Widgets.Add(generator.CreateWidget());
    }

    [RelayCommand]
    public void AddWorkflow()
    {
        if (SelectedProject == null)
            return;
        SelectedProject.AddWorkflow();
    }

    [RelayCommand]
    public async void OpenOrganizerSettings()
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

    [RelayCommand]
    public async void ConfigureWidget(WidgetHost widgetHost)
    {
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
    public void RemoveWidget(object widgetHost)
    {
        if (widgetHost == null)
            return;
        if (!(widgetHost is WidgetHost))
            return;
        SelectedProject.SelectedWorkflow.Widgets.Remove((widgetHost as WidgetHost).DataContext as WidgetContext);
    }

    /// <summary>
    /// Gets the command done configuring widget.
    /// </summary>
    /// <value>The command done configuring widget.</value>
    //public ICommand CommandDoneConfiguringWidget => new RelayCommand(() => ConfiguringWidget = null);

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
        //if (!save)
        //    return;

        //switch (type)
        //{
        //    case DashboardConfigurationType.New:
        //        var dashboardModel = new WorkflowContext { Title = newName };
        //        Workflows.Add(dashboardModel);
        //        SelectedWorkflow = dashboardModel;
        //        return;
        //    case DashboardConfigurationType.Existing:
        //        SelectedWorkflow.Title = newName;
        //        return;
        //}
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

        // --------------------------------------------------------------------------
        // Load Component Data
        // --------------------------------------------------------------------------
        Projects = new ObservableCollection<ProjectContext>
        {
            new ProjectContext { Title = "My Project" }
        };
        SelectedProject = Projects[0];
        SelectedProject.Workflows.Add(new WorkflowContext { Title = "My Workflow" });

        //Workflows = [new WorkflowContext { Title = "My Workflow" }];
        //SelectedWorkflow = Workflows[0];
        AddWidgetMenuItemViewModels = new ObservableCollection<MenuItemViewModel>();

        // --------------------------------------------------------------------------
        // Add Widget Menu
        // --------------------------------------------------------------------------
        AddWidgetMenuItemViewModels.Add(new MenuItemViewModel()
        {
            Header = "Add Widget",
            MenuItems = new ObservableCollection<MenuItemViewModel>(),
        });

        IPackageService packageService = services.GetService<IPackageService>();
        foreach (var widget in packageService.GetAvailableWidgets())
        {
            var fullMenuHeader = widget.MenuInfo.MenuPath;
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
                            SelectedProject.SelectedWorkflow.Widgets.Add(widget.CreateWidget());
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

