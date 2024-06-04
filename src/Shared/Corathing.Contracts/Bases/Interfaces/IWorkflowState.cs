using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Helpers;

namespace Corathing.Contracts.Bases.Interfaces;

public interface IWorkflowSettings
{
    string Name { get; }
}

public class WorkflowSettings : IWorkflowSettings
{
    public string Name { get; set; }
}

public interface IWorkflowState : IEntity
{
    List<WidgetLayout> Layouts { get; }
    IWorkflowSettings Settings { get; }
    int MaxColumns { get; }
    int VisibleRows { get; }
}
