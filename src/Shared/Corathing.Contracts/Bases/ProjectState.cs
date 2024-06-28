using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IProjectCoreState
{
    string Name { get; }
}

public class ProjectCoreState : IProjectCoreState
{
    public string Name { get; set; }
}

public interface IProjectState : IEntity
{
    Guid? SelectedWorkflowId { get; }
    ProjectCoreState CoreSettings { get; }
    List<Guid> WorkflowIds { get; }
}

public class ProjectState : IProjectState
{
    public Guid Id { get; set; }
    public Guid? SelectedWorkflowId { get; set; }
    public ProjectCoreState CoreSettings { get; set; }
    public List<Guid> WorkflowIds { get; set; } = new List<Guid>();

    public static ProjectState Create()
    {
        return new ProjectState
        {
            Id = Guid.NewGuid(),
            CoreSettings = new ProjectCoreState
            {
                Name = "My Project"
            },
            WorkflowIds = new List<Guid>(),
            SelectedWorkflowId = null
        };
    }

    public static ProjectState UpdateProjectSettings(ProjectState project, ProjectCoreState settings)
    {
        return new ProjectState
        {
            Id = project.Id,
            CoreSettings = settings,
            WorkflowIds = project.WorkflowIds,
            SelectedWorkflowId = project.SelectedWorkflowId
        };
    }

    public static ProjectState UpdateProjectWorkflows(ProjectState project, List<Guid> workflowIds)
    {
        return new ProjectState
        {
            Id = project.Id,
            CoreSettings = project.CoreSettings,
            WorkflowIds = workflowIds,
            SelectedWorkflowId = project.SelectedWorkflowId
        };
    }
}
