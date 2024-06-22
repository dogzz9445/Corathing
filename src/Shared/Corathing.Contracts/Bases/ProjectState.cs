using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Helpers;

namespace Corathing.Contracts.Bases;

public class ProjectState : IProjectState
{
    public Guid Id { get; set; }
    public Guid SelectedWorkflowId { get; set; }
    public IProjectSettings Settings { get; set; }
    public List<Guid> WorkflowIds { get; set; }

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
            SelectedWorkflowId = Guid.Empty
        };
    }

    public static ProjectState Create()
    {
        return new ProjectState
        {
            Id = Guid.NewGuid(),
            Settings = new ProjectSettings
            {
                Name = "My Project"
            },
            WorkflowIds = new List<Guid>(),
            SelectedWorkflowId = Guid.Empty
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
            SelectedWorkflowId = project.SelectedWorkflowId
        };
    }

    public static ProjectState UpdateProjectWorkflows(ProjectState project, List<Guid> workflowIds)
    {
        return new ProjectState
        {
            Id = project.Id,
            Settings = project.Settings,
            WorkflowIds = workflowIds,
            SelectedWorkflowId = project.SelectedWorkflowId
        };
    }
}
