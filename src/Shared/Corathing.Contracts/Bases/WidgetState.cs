using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public interface IWidgetCoreState
{
    string Name { get; }
    string Title { get; }
    bool VisibleTitle { get; }
}

public interface IWidgetState : IEntity
{
    IWidgetCoreState CoreSettings { get; }
    object CustomSettings { get; }
    WidgetContext Context { get; }
}

public class WidgetCoreState : IWidgetCoreState
{
    public string Name { get; set; }
    public string Title { get; set; }
    public bool VisibleTitle { get; set; }
}

public class WidgetState : IWidgetState
{
    public Guid Id { get; set; }
    public IWidgetCoreState CoreSettings { get; set; }
    public object CustomSettings { get; set; }
    public WidgetContext Context { get; set; }
}
