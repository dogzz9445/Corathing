using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Helpers;

namespace Corathing.Contracts.Bases;

public class WorkflowState : IWorkflowState
{
    public Guid Id { get; set; }
    public List<WidgetLayout> Layouts { get; set; }
    public IWorkflowSettings Settings { get; set; }
    public int MaxColumns { get; set; } = 16;
    public int VisibleRows { get; set; } = 8;

    public static WorkflowState CreateWorkflow(Guid id, string name)
    {
        return new WorkflowState
        {
            Id = id,
            Layouts = new List<WidgetLayout>(),
            Settings = new WorkflowSettings
            {
                Name = name
            }
        };
    }

    public static string GenerateWorkflowName(List<string> usedNames)
    {
        return NameHelper.GenerateUniqueName("Workflow", usedNames);
    }

    public static WorkflowState UpdateWorkflowSettings(WorkflowState workflow, IWorkflowSettings settings)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            Layouts = workflow.Layouts,
            Settings = settings
        };
    }

    public static WorkflowState UpdateWorkflowLayout(WorkflowState workflow, List<WidgetLayout> layout)
    {
        return new WorkflowState
        {
            Id = workflow.Id,
            Layouts = layout,
            Settings = workflow.Settings
        };
    }
}
