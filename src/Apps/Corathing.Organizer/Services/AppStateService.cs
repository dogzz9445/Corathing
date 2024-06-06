using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

namespace Corathing.Organizer.Services;

public class AppStateService : IAppStateService
{
    public bool TryGetProject(Guid id, out ProjectState project)
    {
        project = new ProjectState();
        return true;
    }

    public bool TryGetWidgetOption<T>(Guid id, out T option)
    {
        option = default;
        return true;
    }

    public bool TryGetWorkflow(Guid id, out WorkflowState workflow)
    {
        workflow = new WorkflowState();
        return true;
    }

    public void Update(string key, string value)
    {
    }

    public void UpdateOrAdd(Guid id, object value)
    {
    }

    public void UpdateOverwrite(Guid id, object value)
    {
    }
}
