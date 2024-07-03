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
    public List<DataSourceState> DataSources { get; set; }
    #endregion

    #region Ignored Serialization Properties    
    [JsonIgnore]
    public Dictionary<Guid, ProjectState> CachedProjects { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WorkflowState> CachedWorkflows { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, WidgetState> CachedWidgets { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, DataSourceState> CachedDataSources { get; set; }
    #endregion

    public AppDashboardState()
    {
        Projects = new List<ProjectState>();
        Workflows = new List<WorkflowState>();
        Widgets = new List<WidgetState>();
        DataSources = new List<DataSourceState>();

        CachedProjects = new Dictionary<Guid, ProjectState>();
        CachedWorkflows = new Dictionary<Guid, WorkflowState>();
        CachedWidgets = new Dictionary<Guid, WidgetState>();
        CachedDataSources = new Dictionary<Guid, DataSourceState>();
    }

    public void RemoveProject(ProjectState project)
    {
        CachedProjects.Remove(project.Id);
        Projects.Remove(project);
    }

    public void RemoveWorkflow(WorkflowState workflow)
    {
        CachedWorkflows.Remove(workflow.Id);
        Workflows.Remove(workflow);
    }

    public void RemoveWidget(WidgetState widget)
    {
        CachedWidgets.Remove(widget.Id);
        Widgets.Remove(widget);
    }

    public void RemoveDataSource(DataSourceState dataSource)
    {
        CachedDataSources.Remove(dataSource.Id);
        DataSources.Remove(dataSource);
    }

    public void UpdateProject(ProjectState project)
    {
        if (CachedProjects.ContainsKey(project.Id))
        {
            var oldProject = CachedProjects[project.Id];
            Projects.RemoveAll(item => item.Id == oldProject.Id);
        }
        CachedProjects[project.Id] = project;
        Projects.Add(project);
    }

    public void UpdateWorkflow(WorkflowState workflow)
    {
        if (CachedWorkflows.ContainsKey(workflow.Id))
        {
            var oldWorkflow = CachedWorkflows[workflow.Id];
            Workflows.RemoveAll(item => item.Id == oldWorkflow.Id);
        }
        CachedWorkflows[workflow.Id] = workflow;
        Workflows.Add(workflow);
    }

    public void UpdateWidget(WidgetState widget)
    {
        if (CachedWidgets.ContainsKey(widget.Id))
        {
            var oldWidget = CachedWidgets[widget.Id];
            Widgets.Remove(oldWidget);
        }
        CachedWidgets[widget.Id] = widget;
        Widgets.Add(widget);
    }

    public void UpdateDataSource(DataSourceState dataSource)
    {
        if (CachedDataSources.ContainsKey(dataSource.Id))
        {
            var oldDataSource = CachedDataSources[dataSource.Id];
            DataSources.Remove(oldDataSource);
        }
        CachedDataSources[dataSource.Id] = dataSource;
        DataSources.Add(dataSource);
    }

    public void RefreshCache()
    {
        CachedProjects.Clear();
        CachedWorkflows.Clear();
        CachedWidgets.Clear();
        CachedDataSources.Clear();

        foreach (var project in Projects)
        {
            CachedProjects[project.Id] = project;
        }
        foreach (var workflow in Workflows)
        {
            CachedWorkflows[workflow.Id] = workflow;
        }
        foreach (var widget in Widgets)
        {
            CachedWidgets[widget.Id] = widget;
        }
        foreach (var dataSource in DataSources)
        {
            CachedDataSources[dataSource.Id] = dataSource;
        }
    }
}
