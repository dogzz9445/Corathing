using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Models;
using Corathing.Organizer.Services;
using Corathing.Organizer.Views;

using Microsoft.Extensions.DependencyInjection;

using NuGet.Packaging;

using Wpf.Ui;

using static MaterialDesignThemes.Wpf.Theme.ToolBar;

using Application = System.Windows.Application;

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
    private IServiceProvider _services;

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

    #region Contructors
    public DashboardViewModel()
    {
        Projects = new ObservableCollection<ProjectContext>();
        AddWidgetMenuItemViewModels = new ObservableCollection<MenuItemViewModel>();
    }
    #endregion

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
    public void ToggleEditDashboard()
    {
        EditMode = !EditMode;
    }

    [RelayCommand]
    public void AddWidget(CoraWidgetGenerator generator)
    {
        if (SelectedProject == null)
            return;
        if (SelectedProject.SelectedWorkflow == null)
            return;
        SelectedProject.SelectedWorkflow.AddWidget(generator);
    }

    [RelayCommand]
    public void AddWorkflow()
    {
        if (SelectedProject == null)
            return;
        SelectedProject.AddWorkflow();
    }

    [RelayCommand]
    public void AddProject()
    {
        var projectContext = ProjectContext.Create();
        Projects.Add(projectContext);
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
        Application.Current.MainWindow.Effect = new BlurEffect();
        window.ShowDialog();
        Application.Current.MainWindow.Effect = null;
    }

    [RelayCommand]
    public void OpenWorkflowSettings()
    {
        var window = new BaseWindow();
        window.Content = new WorkflowSettingsView();
        window.Owner = Application.Current.MainWindow;
        Application.Current.MainWindow.Effect = new BlurEffect();
        window.ShowDialog();
        Application.Current.MainWindow.Effect = null;
    }

    [RelayCommand]
    public void RemoveWorkflow()
    {

    }

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
        _services = services;

        UpdateAvailableWidgets();
        UpdateDashboard();

        return Task.CompletedTask;
    }

    private void UpdateDashboard()
    {
        Projects.Clear();

        var packageService = _services.GetService<IPackageService>();
        var appStateService = _services.GetService<IAppStateService>();
        var dashboardState = appStateService.GetAppDashboardState();

        foreach (var projectState in dashboardState.Projects)
        {
            var projectContext = _services.GetService<ProjectContext>();
            projectContext.Name = projectState.Settings.Name;
            projectContext.EditMode = EditMode;
            projectContext.UpdateProject(projectState);

            Projects.Add(projectContext);
        }
        if (dashboardState.SelectedProjectId != null)
        {
            SelectedProject = Projects.FirstOrDefault(context => context.ProjectId == dashboardState.SelectedProjectId);
        }
        if (SelectedProject == null)
        { 
            SelectedProject = Projects.FirstOrDefault();
        }
    }

    private void UpdateAvailableWidgets()
    {
        AddWidgetMenuItemViewModels.Clear();
        AddWidgetMenuItemViewModels.Add(new MenuItemViewModel()
        {
            Header = "Add Widget",
            MenuItems = new ObservableCollection<MenuItemViewModel>(),
        });

        IPackageService packageService = _services.GetService<IPackageService>();
        foreach (var widget in packageService.GetAvailableWidgets())
        {
            var fullMenuHeader = widget.Info.MenuPath;
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
                        Command = new RelayCommand(() => AddWidget(widget), () => true),
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
    }

    #endregion Public Methods
}

