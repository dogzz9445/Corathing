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

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.Models;

public partial class ProjectContext : ObservableObject
{
    #region Readonly Properties
    private IServiceProvider _services;
    private ProjectState _projectState;
    #endregion
    #region Public Properties
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _title = "My Project";

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
        // FIXME:
        // 적용되게 수정
        var appState = _services.GetService<IAppStateService>();
        var workflow = appState.AddWorkflow();
        //appState.NewWorkflowState();
        var workflowContext = _services.GetService<WorkflowContext>();
        workflowContext.WorkflowId = workflow.Id;
        workflowContext.EditMode = EditMode;
        workflowContext.WorkflowId = Guid.NewGuid();

        Workflows.Add(workflowContext);
        SelectedWorkflow = workflowContext;
    }

    #endregion

    public ProjectContext(IServiceProvider services)
    {
        _services = services;
        Workflows = new ObservableCollection<WorkflowContext>();
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
