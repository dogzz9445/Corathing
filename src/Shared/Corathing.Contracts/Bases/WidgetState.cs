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
    WidgetCoreState CoreSettings { get; }
    object CustomSettings { get; }
}

public class WidgetState : IWidgetState
{
    public Guid Id { get; set; }
    public WidgetCoreState CoreSettings { get; set; }
    public object CustomSettings { get; set; }
}
