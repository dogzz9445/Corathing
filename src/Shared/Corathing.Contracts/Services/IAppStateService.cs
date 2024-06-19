using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Services;

public interface IAppStateService
{
    Task InitializeAsync();
    AppSettings GetAppSettings();
    void UpdateAppSettings(AppSettings appSettings);
    AppPreferenceState GetAppPreferenceState();
    AppPackageState GetAppPackageState();
    AppDashboardState GetAppDashboardState();

    bool TryGetProject(Guid id, out ProjectState project);
    bool TryGetWorkflow(Guid id, out WorkflowState workflow);
    bool TryGetWidget(Guid id, out WidgetState widget);

    //ProjectState GetOrAddProject(Guid? projectId = null);
    //IEnumerable<WidgetState> GetWidgets(Guid? workflowId = null);

    void UpdateProject(ProjectState project);
    void UpdateWorkflow(WorkflowState workflow);
    void UpdateWidget(WidgetState widget);
}
