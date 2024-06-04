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
    IProjectSettings Settings { get; }
    List<Guid> WorkflowIds { get; }
    Guid CurrentWorkflowId { get; }
}

public class ProjectState : IProjectState
{
    public Guid Id { get; set; }
    public IProjectSettings Settings { get; set; }
    public List<Guid> WorkflowIds { get; set; }
    public Guid CurrentWorkflowId { get; set; }

    public static ProjectState CreateProject(Guid id, string projectName)
    {
        return new ProjectState
        {
            Id = id,
            Settings = new ProjectSettings
            {
                Name = projectName
            },
            WorkflowIds = new List<Guid>(),
            CurrentWorkflowId = Guid.Empty
        };
    }

    public static string GenerateProjectName(List<string> usedNames)
    {
        return NameHelper.GenerateUniqueName("Project", usedNames);
    }

    public static ProjectState UpdateProjectSettings(ProjectState project, IProjectSettings settings)
    {
        return new ProjectState
        {
            Id = project.Id,
            Settings = settings,
            WorkflowIds = project.WorkflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }

    public static ProjectState UpdateProjectWorkflows(ProjectState project, List<Guid> workflowIds)
    {
        return new ProjectState
        {
            Id = project.Id,
            Settings = project.Settings,
            WorkflowIds = workflowIds,
            CurrentWorkflowId = project.CurrentWorkflowId
        };
    }
}
