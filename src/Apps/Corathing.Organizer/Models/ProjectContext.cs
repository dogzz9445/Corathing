using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

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

    /// <summary>
    /// Gets or sets the workflows.
    /// </summary>
    /// <value>The widgets.</value>
    [ObservableProperty]
    private ObservableCollection<WorkflowContext> _workflows;
    #endregion

    public ProjectContext()
    {
        Workflows = new ObservableCollection<WorkflowContext>();
    }
}
