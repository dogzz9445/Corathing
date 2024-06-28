using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IWorkflowCoreState
{
    string Name { get; }
}

public class WorkflowCoreState : IWorkflowCoreState
{
    public string Name { get; set; }
}

public interface IWorkflowState : IEntity
{
    public List<Guid> WidgetIds { get; set; }
    WorkflowCoreState CoreSettings { get; }
    int MaxColumns { get; }
    int VisibleRows { get; }
}

public class WorkflowState : IWorkflowState
{
    public Guid Id { get; set; }
    public List<Guid> WidgetIds { get; set; } = new List<Guid>();
    public WorkflowCoreState CoreSettings { get; set; }
    public int MaxColumns { get; set; } = 16;
    public int VisibleRows { get; set; } = 8;

    public static WorkflowState CreateWorkflow(Guid id, string name)
    {
        return new WorkflowState
        {
            Id = id,
            CoreSettings = new WorkflowCoreState
            {
                Name = name
            }
        };
    }

    public static WorkflowState Create()
        => new WorkflowState
        {
            Id = Guid.NewGuid(),
            CoreSettings = new WorkflowCoreState
            {
                Name = "My Workflow"
            }
        };

    public static WorkflowState UpdateWorkflowSettings(WorkflowState workflow, WorkflowCoreState settings)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            CoreSettings = settings
        };
    }

    public static WorkflowState UpdateWorkflowLayout(WorkflowState workflow, List<WidgetLayout> layout)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            CoreSettings = workflow.CoreSettings
        };
    }
}
