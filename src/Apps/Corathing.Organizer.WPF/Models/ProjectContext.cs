using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Organizer.WPF.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Models;

public partial class ProjectContext : ObservableObject
{
    #region Readonly Properties
    private IServiceProvider _services;
    #endregion
    #region Public Properties
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _name = "My Project";

    [DefaultValue(false)]
    [ObservableProperty]
    private bool? _editMode;

    /// <summary>
    /// Gets or sets the dashboards.
    /// </summary>
    /// <value>The dashboards.</value>
    private ObservableCollection<WorkflowContext>? _workflows;
    public ObservableCollection<WorkflowContext> Workflows
    {
        get => _workflows;
        set
        {
            if (EqualityComparer<ObservableCollection<WorkflowContext>?>.Default.Equals(_workflows, value))
                return;
            OnPropertyChanging(nameof(Workflows));
            _workflows = value;
            var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_workflows);
            itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            OnPropertyChanged(nameof(Workflows));
        }
    }

    [ObservableProperty]
    private WorkflowContext _selectedWorkflow;

    [RelayCommand]
    public void AddWorkflow()
    {
        var appState = _services.GetService<IAppStateService>();

        var workflowState = appState.CreateAddWorkflow();
        var workflowContext = _services.GetService<WorkflowContext>();
        workflowContext.EditMode = EditMode;
        workflowContext.UpdateWorkflow(workflowState);

        if (!appState.TryGetProject(ProjectId, out var projectState))
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        projectState.WorkflowIds.Add(workflowState.Id);
        appState.UpdateProject(projectState);

        Workflows.Add(workflowContext);
        SelectedWorkflow = workflowContext;
    }

    #endregion

    public ProjectContext(IServiceProvider services)
    {
        _services = services;
        Workflows = new ObservableCollection<WorkflowContext>();
    }

    public static ProjectContext Create(ProjectState? state = null)
    {
        if (state == null)
        {
            var appStateService = App.Current.Services.GetService<IAppStateService>();
            state = appStateService.CreateAddProject();
        }
        var context = App.Current.Services.GetService<ProjectContext>();
        context.Name = state.CoreSettings.Name;

        return context;
    }

    public void UpdateProject(ProjectState projectState)
    {
        var appStateService = _services.GetService<IAppStateService>();

        ProjectId = projectState.Id;
        Name = projectState.CoreSettings.Name;

        foreach (var workflowStateId in projectState.WorkflowIds)
        {
            if (!appStateService.TryGetWorkflow(workflowStateId, out var workflowState))
            {
                // TODO:
                // Change Exception Type
                throw new Exception();
            }
            var workflowContext = _services.GetService<WorkflowContext>();
            workflowContext.EditMode = EditMode;
            workflowContext.UpdateWorkflow(workflowState);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(EditMode))
        {
            foreach (var workflow in Workflows)
            {
                workflow.EditMode = EditMode;
            }
        }
    }
}
