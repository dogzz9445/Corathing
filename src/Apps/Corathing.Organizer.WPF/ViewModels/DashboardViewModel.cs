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
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.WPF.Controls;
using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.Models;
using Corathing.Organizer.WPF.Services;
using Corathing.Organizer.WPF.Views;

using Microsoft.Extensions.DependencyInjection;

using NuGet.Packaging;

using Wpf.Ui;

using static MaterialDesignThemes.Wpf.Theme.ToolBar;

using Application = System.Windows.Application;

namespace Corathing.Organizer.WPF.ViewModels;

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
    public void AddWidget(ICoraWidgetInfo info)
    {
        if (SelectedProject == null)
            return;
        if (SelectedProject.SelectedWorkflow == null)
            return;
        SelectedProject.SelectedWorkflow.AddWidget(info);
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
        Projects?.Add(projectContext);
    }

    [RelayCommand]
    public async Task OpenOrganizerSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        await navigationService.Navigate<OrganizerSettingsView>();
    }

    [RelayCommand]
    public async Task OpenProjectSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        await navigationService.Navigate<ProjectSettingsView>();
    }

    [RelayCommand]
    public async Task OpenWorkflowSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        await navigationService.Navigate<WorkflowSettingsView>();
    }

    [RelayCommand]
    public async Task OpenPackageManagementView()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        await navigationService.Navigate<PackageManagementView>();
    }

    [RelayCommand]
    public async Task ConfigureWidget(WidgetHost widget)
    {
        if (SelectedProject == null)
            return;
        if (SelectedProject.SelectedWorkflow == null)
            return;

        await SelectedProject.SelectedWorkflow.ConfigureWidget(widget);
    }

    [RelayCommand]
    public void RemoveWidget(WidgetHost widget)
    {
        SelectedProject?.SelectedWorkflow?.RemoveWidget(widget);
    }

    [RelayCommand]
    public void ConfigureWorkflow(WorkflowContext context)
    {
        //SelectedProject?.SelectedWorkflow?.ConfigureWorkflow();
    }

    [RelayCommand]
    public void RemoveWorkflow(WorkflowContext context)
    {
        SelectedProject?.RemoveWorkflow(context);
    }

    #endregion Public Properties

    #region Public Methods

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
        foreach (var projectContext in Projects)
        {
            projectContext.Destroy();
        }
        Projects.Clear();

        var packageService = _services.GetService<IPackageService>();
        var appStateService = _services.GetService<IAppStateService>();
        var dashboardState = appStateService.GetAppDashboardState();

        foreach (var projectState in dashboardState.Projects)
        {
            var projectContext = _services.GetService<ProjectContext>();
            projectContext.Name = projectState.CoreSettings.Name;
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

        foreach (var projectContext in Projects)
        {
            if (!appStateService.TryGetProject(projectContext.ProjectId, out var projectState))
            {
                // TODO:
                // Change Exception Type
                throw new Exception();
            }
            if (projectState.SelectedWorkflowId != null)
            {
                projectContext.SelectedWorkflow = projectContext.Workflows.FirstOrDefault(context => context.WorkflowId == projectState.SelectedWorkflowId);
            }
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
        foreach (var info in packageService.GetAvailableWidgets())
        {
            var fullMenuHeader = info.MenuPath;
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
                        Command = new RelayCommand(() => AddWidget(info), () => true),
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

