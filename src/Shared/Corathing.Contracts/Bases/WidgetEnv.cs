using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IWidgetEnvCommon
{
    bool? IsPreview { get; }
}

public interface IWidgetEnvAreaShelf : IWidgetEnvCommon
{
    string Area { get; }
}

public interface IWidgetEnvAreaWorkflow : IWidgetEnvCommon
{
    string Area { get; }
    Guid ProjectId { get; }
    Guid WorkflowId { get; }
}

public interface IWidgetEnvState : IWidgetEnvAreaShelf, IWidgetEnvAreaWorkflow
{
    // This interface can be used as a marker interface for WidgetEnvAreaShelf and WidgetEnvAreaWorkflow
}

public class WidgetEnvAreaShelf : IWidgetEnvAreaShelf
{
    public string Area { get; set; }
    public bool? IsPreview { get; set; }
}

public class WidgetEnvAreaWorkflow : IWidgetEnvAreaWorkflow
{
    public string Area { get; set; }
    public bool? IsPreview { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WorkflowId { get; set; }
}

public class WidgetEnv<TSettings>
{
    public IWidgetEnvState Env { get; set; }
    public IWidgetState<TSettings> Widget { get; set; }
}
