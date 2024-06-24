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
    public Guid? SelectedWorkflowId { get; set; }
    public ProjectSettings Settings { get; set; }
    public List<Guid> WorkflowIds { get; set; } = new List<Guid>();

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
            SelectedWorkflowId = null
        };
    }

    public static string GenerateProjectName(List<string> usedNames)
    {
        return NameHelper.GenerateUniqueName("Project", usedNames);
    }

    public static ProjectState UpdateProjectSettings(ProjectState project, ProjectSettings settings)
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
