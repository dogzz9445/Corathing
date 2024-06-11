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
    public Dictionary<Guid, WidgetState<StateRecord>> Widgets { get; set; }
}
