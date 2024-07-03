using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Configurations;
using Corathing.Contracts.DataContexts;

namespace Corathing.Contracts.Services;

public interface IAppStateService
{
    Task InitializeAsync();

    AppSettings GetAppSettings();
    void UpdateAppSettings(AppSettings appSettings);

    AppPreferenceState GetAppPreferenceState();
    AppPackageState GetAppPackageState();
    AppDashboardState GetAppDashboardState();

    // Dashboards (Projects, Workflows, Widgets)
    ProjectState CreateAddProject();
    WorkflowState CreateAddWorkflow();

    ProjectState CloneProject(Guid originalProjectId);
    ProjectState CloneProject(ProjectState originalProject);
    WorkflowState CloneWorkflow(Guid originalWorkflowId);
    WorkflowState CloneWorkflow(WorkflowState originalWorkflow);

    bool TryGetPackage(Guid id, out PackageState package);
    bool TryGetDataSource(Guid id, out DataSourceState dataSource);
    bool TryGetProject(Guid id, out ProjectState project);
    bool TryGetWorkflow(Guid id, out WorkflowState workflow);
    bool TryGetWidget(Guid id, out WidgetState widget);

    void UpdatePackage(PackageState package);
    void UpdateDataSource(DataSourceState dataSource);
    void UpdateProject(ProjectState project);
    void UpdateWorkflow(WorkflowState workflow);
    void UpdateWidget(WidgetState widget);
    void UpdateForce();

    void RemovePackage(PackageState package, bool removeReferencedWidgets = false);
    void RemovePackage(Guid packageId, bool removeReferencedWidgets = false);
    void RemoveDataSource(DataSourceState dataSource);
    void RemoveDataSource(Guid dataSourceId);
    void RemoveProject(ProjectState project);
    void RemoveProject(Guid projectId);
    void RemoveWorkflow(WorkflowState workflow);
    void RemoveWorkflow(Guid workflowId);
    void RemoveWidget(WidgetState widget);
    void RemoveWidget(Guid widgetId);

    void Flush();
}
