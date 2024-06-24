using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Organizer.Models;
using Corathing.Organizer.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.ViewModels;

public partial class ProjectSettingsViewModel : ObservableObject
{
    private IServiceProvider _services;
    [ObservableProperty]
    private ObservableCollection<ProjectSettingsContext> _projectSettingsContexts;

    [ObservableProperty]
    private ProjectSettingsContext _selectedProjectContext;

    public ProjectSettingsViewModel(IServiceProvider services)
    {
        _services = services;
        ProjectSettingsContexts = new ObservableCollection<ProjectSettingsContext>();

        var appStateService = _services.GetService<IAppStateService>();
        var dashboards = appStateService.GetAppDashboardState();

        foreach (var projectState in dashboards.Projects)
        {
            var projectSettingsContext = _services.GetService<ProjectSettingsContext>();
            projectSettingsContext.Name = projectState.Settings.Name;
            projectSettingsContext.ProjectId = projectState.Id;
            projectSettingsContext.ProjectState = projectState;
            ProjectSettingsContexts.Add(projectSettingsContext);
        }
    }

    [RelayCommand]
    public void AddProject()
    {
        var projectSettingsContext = _services.GetService<ProjectSettingsContext>();
        projectSettingsContext.IsAdded = true;
        ProjectSettingsContexts.Add(projectSettingsContext);
    }

    [RelayCommand]
    public void DuplicateProject(ProjectSettingsContext originalContext)
    {
        var projectSettingsContext = _services.GetService<ProjectSettingsContext>();
        projectSettingsContext.IsDuplicated = true;
        projectSettingsContext.Name = $"{originalContext.Name} Copy";
        projectSettingsContext.OriginalProjectId = originalContext.ProjectId;
        ProjectSettingsContexts.Add(projectSettingsContext);
    }

    [RelayCommand]
    public void RemoveProject(ProjectSettingsContext projectSettingsContext)
    {
        projectSettingsContext.IsRemoved = true;
        ProjectSettingsContexts.Remove(projectSettingsContext);
    }

    [RelayCommand]
    public void Apply()
    {
        var appStateService = _services.GetService<IAppStateService>();

        foreach (var projectSettingsContext in ProjectSettingsContexts)
        {
            if (projectSettingsContext.IsRemoved)
            {
                appStateService.RemoveProject(projectSettingsContext.ProjectId);
            }
            else if (projectSettingsContext.IsAdded)
            {
                var projectState = ProjectState.Create();
                projectState.Settings.Name = projectSettingsContext.Name;
                appStateService.UpdateProject(projectState);
            }
            else if (projectSettingsContext.IsDuplicated)
            {
                var projectState = CopyProject(projectSettingsContext.OriginalProjectId);
                projectState.Settings.Name = projectSettingsContext.Name;
                appStateService.UpdateProject(projectState);
            }
            else
            {
                if (projectSettingsContext.ProjectState.Settings.Name != projectSettingsContext.Name)
                {
                    projectSettingsContext.ProjectState.Settings.Name = projectSettingsContext.Name;
                    appStateService.UpdateProject(projectSettingsContext.ProjectState);
                }
            }
        }

        appStateService.Flush();
    }

    private ProjectState CopyProject(Guid originalProjectId)
    {
        var originalProjectSettingsContext = ProjectSettingsContexts.FirstOrDefault(context => context.ProjectId == originalProjectId);
        if (originalProjectSettingsContext == null)
        {
            // TODO:
            // Change Exception Type
            throw new InvalidOperationException();
        }
        if (originalProjectSettingsContext.IsAdded)
        {
            return ProjectState.Create();
        }
        if (originalProjectSettingsContext.IsDuplicated)
        {
            return CopyProject(originalProjectSettingsContext.OriginalProjectId);
        }
        var appStateService = _services.GetService<IAppStateService>();
        return appStateService.CopyProject(originalProjectSettingsContext.ProjectId);
    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
        var navigationService = _services.GetService<INavigationService>();

        // TODO:
        // navigationService.GoBack();
    }

}
