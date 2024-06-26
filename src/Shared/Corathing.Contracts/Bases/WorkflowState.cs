using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public class WorkflowState : IWorkflowState
{
    public Guid Id { get; set; }
    public List<Guid> WidgetIds { get; set; } = new List<Guid>();
    public WorkflowSettings Settings { get; set; }
    public int MaxColumns { get; set; } = 16;
    public int VisibleRows { get; set; } = 8;

    public static WorkflowState CreateWorkflow(Guid id, string name)
    {
        return new WorkflowState
        {
            Id = id,
            Settings = new WorkflowSettings
            {
                Name = name
            }
        };
    }

    public static WorkflowState Create()
        => new WorkflowState
        {
            Id = Guid.NewGuid(),
            Settings = new WorkflowSettings
            {
                Name = "My Workflow"
            }
        };

    public static WorkflowState UpdateWorkflowSettings(WorkflowState workflow, WorkflowSettings settings)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            Settings = settings
        };
    }

    public static WorkflowState UpdateWorkflowLayout(WorkflowState workflow, List<WidgetLayout> layout)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            Settings = workflow.Settings
        };
    }
}
