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

    bool TryGetWorkflow(Guid id, out WorkflowState workflow);
    bool TryGetProject(Guid id, out ProjectState project);
    bool TryGetWidget<T>(Guid id, out T option);
}
