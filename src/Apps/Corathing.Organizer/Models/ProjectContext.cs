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

namespace Corathing.Organizer.Models;

public partial class ProjectContext : ObservableObject
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
            //var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_workflows);
            //itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            OnPropertyChanged(nameof(Workflows));
        }
    }

    [ObservableProperty]
    private WorkflowContext _selectedWorkflow;

    [RelayCommand]
    public void AddWorkflow()
    {
        var newWorkflow = new WorkflowContext();
        Workflows.Add(newWorkflow);
        SelectedWorkflow = newWorkflow;
    }

    #endregion

    public ProjectContext()
    {
        Workflows = new ObservableCollection<WorkflowContext>();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (Workflows != null)
        {
            foreach (var workflow in Workflows)
            {
                workflow.EditMode = EditMode;
            }
        }
    }
}
