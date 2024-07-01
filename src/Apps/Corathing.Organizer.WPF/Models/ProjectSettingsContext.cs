using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Bases;

namespace Corathing.Organizer.WPF.Models;

public partial class ProjectSettingsContext : ObservableObject
{
    #region Readonly Properties
    private IServiceProvider _services;
    #endregion
    public Guid ProjectId { get; set; }
    public Guid OriginalProjectId { get; set; }
    public bool IsAdded { get; set; }
    public bool IsRemoved { get; set; }
    public bool IsDuplicated { get; set; }
    public ProjectState ProjectState { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _name = "My Project";

    public ProjectSettingsContext(IServiceProvider services)
    {
        _services = services;
    }
}
