using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

namespace Corathing.Contracts.Configurations;

public record StateRecord(string key, string value);

public class AppDashboardState
{
    #region Serialized Properties
    public Guid Id { get; set; }

    public Guid? SelectedProjectId { get; set; }

    public List<ProjectState> Projects { get; set; }
    public List<WorkflowState> Workflows { get; set; }
    public List<WidgetState> Widgets { get; set; }
    #endregion

    #region Ignored Serialization Properties    
    [JsonIgnore]
    public Dictionary<Guid, ProjectState> HashedProjects { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WorkflowState> HashedWorkflows { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WidgetState> HashedWidgets { get; set; }
    #endregion

    public AppDashboardState()
    {
        Id = Guid.NewGuid();
        Projects = new List<ProjectState>();
        Workflows = new List<WorkflowState>();
        Widgets = new List<WidgetState>();

        HashedProjects = new Dictionary<Guid, ProjectState>();
        HashedWorkflows = new Dictionary<Guid, WorkflowState>();
        HashedWidgets = new Dictionary<Guid, WidgetState>();
    }

    public ProjectState AddProject(ProjectState project)
    {
        HashedProjects[project.Id] = project;
        Projects.Add(project);
        return project;
    }

    public WorkflowState AddWorkflow(WorkflowState workflow)
    {
        HashedWorkflows[workflow.Id] = workflow;
        Workflows.Add(workflow);
        return workflow;
    }

    public WidgetState AddWidget(WidgetState widget)
    {
        HashedWidgets[widget.Id] = widget;
        Widgets.Add(widget);
        return widget;
    }

    public void RemoveProject(ProjectState project)
    {
        HashedProjects.Remove(project.Id);
        Projects.Remove(project);
    }

    public void RemoveWorkflow(WorkflowState workflow)
    {
        HashedWorkflows.Remove(workflow.Id);
        Workflows.Remove(workflow);
    }

    public void RemoveWidget(WidgetState widget)
    {
        HashedWidgets.Remove(widget.Id);
        Widgets.Remove(widget);
    }

    public void UpdateProject(ProjectState project)
    {
        if (HashedProjects.ContainsKey(project.Id))
        {
            var oldProject = HashedProjects[project.Id];
            Projects.RemoveAll(item => item.Id == oldProject.Id);
        }
        HashedProjects[project.Id] = project;
        Projects.Add(project);
    }

    public void UpdateWorkflow(WorkflowState workflow)
    {
        if (HashedWorkflows.ContainsKey(workflow.Id))
        {
            var oldWorkflow = HashedWorkflows[workflow.Id];
            Workflows.RemoveAll(item => item.Id == oldWorkflow.Id);
        }
        HashedWorkflows[workflow.Id] = workflow;
        Workflows.Add(workflow);
    }

    public void UpdateWidget(WidgetState widget)
    {
        if (HashedWidgets.ContainsKey(widget.Id))
        {
            var oldWidget = HashedWidgets[widget.Id];
            Widgets.Remove(oldWidget);
        }
        HashedWidgets[widget.Id] = widget;
        Widgets.Add(widget);
    }

    public void RefreshCache()
    {
        HashedProjects.Clear();
        HashedWorkflows.Clear();
        HashedWidgets.Clear();

        foreach (var project in Projects)
        {
            HashedProjects[project.Id] = project;
        }
        foreach (var workflow in Workflows)
        {
            HashedWorkflows[workflow.Id] = workflow;
        }
        foreach (var widget in Widgets)
        {
            HashedWidgets[widget.Id] = widget;
        }
    }
}
