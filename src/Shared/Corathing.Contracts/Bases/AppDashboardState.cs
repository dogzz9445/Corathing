using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Bases;

public record StateRecord(string key, string value);

public class AppDashboardState
{
    public Guid Id { get; set; }
    public Dictionary<Guid, ProjectState> Projects { get; set; }
    public Dictionary<Guid, WorkflowState> Workflows { get; set; }
    public Dictionary<Guid, WidgetState> Widgets { get; set; }

    public ProjectState AddProject()
    {
        if (Projects == null)
            Projects = new Dictionary<Guid, ProjectState>();

        var project = new ProjectState();
        Projects[project.Id] = project;
        return project;
    }

    public WorkflowState AddWorkflow()
    {
        if (Workflows == null)
            Workflows = new Dictionary<Guid, WorkflowState>();

        var workflow = new WorkflowState();
        Workflows[workflow.Id] = workflow;
        return workflow;
    }

    public void UpdateProject(ProjectState project)
    {
        if (Projects == null)
            Projects = new Dictionary<Guid, ProjectState>();

        Projects[project.Id] = project;
    }

    public void UpdateWorkflow(WorkflowState workflow)
    {
        if (Workflows == null)
            Workflows = new Dictionary<Guid, WorkflowState>();

        Workflows[workflow.Id] = workflow;
    }

    public void UpdateWidget(WidgetState widget)
    {
        if (Widgets == null)
            Widgets = new Dictionary<Guid, WidgetState>();

        Widgets[widget.Id] = widget;
    }
}
