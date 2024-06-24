using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Helpers;

namespace Corathing.Contracts.Bases.Interfaces;

public interface IProjectSettings
{
    string Name { get; }
}

public class ProjectSettings : IProjectSettings
{
    public string Name { get; set; }
}

public interface IProjectState : IEntity
{
    Guid? SelectedWorkflowId { get; }
    ProjectSettings Settings { get; }
    List<Guid> WorkflowIds { get; }
}
