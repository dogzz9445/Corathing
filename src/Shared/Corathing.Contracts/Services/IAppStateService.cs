using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Configurations;

namespace Corathing.Contracts.Services;

public interface IAppStateService
{
    Task InitializeAsync();


    AppSettings GetAppSettings();
    void UpdateAppSettings(AppSettings appSettings);

    AppPreferenceState GetAppPreferenceState();
    AppPackageState GetAppPackageState();
    AppDashboardState GetAppDashboardState();

    ProjectState CreateAddProject();
    WorkflowState CreateAddWorkflow();

    ProjectState CopyProject(Guid originalProjectId);
    ProjectState CopyProject(ProjectState originalProject);
    WorkflowState CopyWorkflow(Guid originalWorkflowId);
    WorkflowState CopyWorkflow(WorkflowState originalWorkflow);

    void RemoveProject(ProjectState project);
    void RemoveProject(Guid projectId);
    void RemoveWorkflow(WorkflowState workflow);
    void RemoveWorkflow(Guid workflowId);
    void RemoveWidget(WidgetState widget);
    void RemoveWidget(Guid widgetId);

    bool TryGetProject(Guid id, out ProjectState project);
    bool TryGetWorkflow(Guid id, out WorkflowState workflow);
    bool TryGetWidget(Guid id, out WidgetState widget);

    void UpdateProject(ProjectState project);
    void UpdateWorkflow(WorkflowState workflow);
    void UpdateWidget(WidgetState widget);
    void UpdateForce();

    void Flush();
}
