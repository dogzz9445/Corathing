using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Services;

public interface IAppStateService
{
    void Update(string key, string value);
    void UpdateOverwrite(Guid id, object value);
    void UpdateOrAdd(Guid id, object value);
    bool TryGetWorkflow(Guid id, out WorkflowState workflow);
    bool TryGetProject(Guid id, out ProjectState project);
    bool TryGetWidgetOption<T>(Guid id, out T option);
}
