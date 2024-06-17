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
    public Dictionary<Guid, ProjectState> Projects { get; set; }
    public Dictionary<Guid, WorkflowState> Workflows { get; set; }
    public Dictionary<Guid, WidgetState> Widgets { get; set; }
}
