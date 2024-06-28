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

public class AppDashboardState : IEntity
{
    #region Serialized Properties
    public Guid Id { get; set; }

    public Guid? SelectedProjectId { get; set; }

    public List<ProjectState> Projects { get; set; }
    public List<WorkflowState> Workflows { get; set; }
    public List<WidgetState> Widgets { get; set; }
    public List<DataSourceState> DataSourceStates { get; set; }
    #endregion

    #region Ignored Serialization Properties    
    [JsonIgnore]
    public Dictionary<Guid, ProjectState> CashedProjects { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WorkflowState> CashedWorkflows { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WidgetState> CashedWidgets { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, DataSourceState> CashedDataSources { get; set; }
    #endregion

    public AppDashboardState()
    {
        Id = Guid.NewGuid();
        Projects = new List<ProjectState>();
        Workflows = new List<WorkflowState>();
        Widgets = new List<WidgetState>();
        DataSourceStates = new List<DataSourceState>();

        CashedProjects = new Dictionary<Guid, ProjectState>();
        CashedWorkflows = new Dictionary<Guid, WorkflowState>();
        CashedWidgets = new Dictionary<Guid, WidgetState>();
        CashedDataSources = new Dictionary<Guid, DataSourceState>();
    }

    public void RemoveProject(ProjectState project)
    {
        CashedProjects.Remove(project.Id);
        Projects.Remove(project);
    }

    public void RemoveWorkflow(WorkflowState workflow)
    {
        CashedWorkflows.Remove(workflow.Id);
        Workflows.Remove(workflow);
    }

    public void RemoveWidget(WidgetState widget)
    {
        CashedWidgets.Remove(widget.Id);
        Widgets.Remove(widget);
    }

    public void RemoveDataSource(DataSourceState dataSource)
    {
        CashedDataSources.Remove(dataSource.Id);
        DataSourceStates.Remove(dataSource);
    }

    public void UpdateProject(ProjectState project)
    {
        if (CashedProjects.ContainsKey(project.Id))
        {
            var oldProject = CashedProjects[project.Id];
            Projects.RemoveAll(item => item.Id == oldProject.Id);
        }
        CashedProjects[project.Id] = project;
        Projects.Add(project);
    }

    public void UpdateWorkflow(WorkflowState workflow)
    {
        if (CashedWorkflows.ContainsKey(workflow.Id))
        {
            var oldWorkflow = CashedWorkflows[workflow.Id];
            Workflows.RemoveAll(item => item.Id == oldWorkflow.Id);
        }
        CashedWorkflows[workflow.Id] = workflow;
        Workflows.Add(workflow);
    }

    public void UpdateWidget(WidgetState widget)
    {
        if (CashedWidgets.ContainsKey(widget.Id))
        {
            var oldWidget = CashedWidgets[widget.Id];
            Widgets.Remove(oldWidget);
        }
        CashedWidgets[widget.Id] = widget;
        Widgets.Add(widget);
    }

    public void UpdateDataSource(DataSourceState dataSource)
    {
        if (CashedDataSources.ContainsKey(dataSource.Id))
        {
            var oldDataSource = CashedDataSources[dataSource.Id];
            DataSourceStates.Remove(oldDataSource);
        }
        CashedDataSources[dataSource.Id] = dataSource;
        DataSourceStates.Add(dataSource);
    }

    public void RefreshCache()
    {
        CashedProjects.Clear();
        CashedWorkflows.Clear();
        CashedWidgets.Clear();
        CashedDataSources.Clear();

        foreach (var project in Projects)
        {
            CashedProjects[project.Id] = project;
        }
        foreach (var workflow in Workflows)
        {
            CashedWorkflows[workflow.Id] = workflow;
        }
        foreach (var widget in Widgets)
        {
            CashedWidgets[widget.Id] = widget;
        }
        foreach (var dataSource in DataSourceStates)
        {
            CashedDataSources[dataSource.Id] = dataSource;
        }
    }
}
