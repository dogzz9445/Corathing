using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public interface IWidgetState : IEntity
{
    IWidgetCoreState CoreSettings { get; }
    List<object> CustomSettings { get; }
    WidgetContext Context { get; }
}

public class WidgetState : IWidgetState
{
    public Guid Id { get; set; }
    public IWidgetCoreState CoreSettings { get; set; }
    public List<object> CustomSettings { get; set; }
    public WidgetContext Context { get; set; }
}
