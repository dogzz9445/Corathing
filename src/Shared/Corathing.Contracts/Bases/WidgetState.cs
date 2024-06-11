using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public interface IWidgetState<TSettings> : IEntity
{
    IWidgetCoreState CoreSettings { get; }
    List<TSettings> Settings { get; }
    WidgetContext Context { get; }
}

public class WidgetState<TSettings> : IWidgetState<TSettings>
{
    public Guid Id { get; set; }
    public IWidgetCoreState CoreSettings { get; set; }
    public List<TSettings> Settings { get; set; }
    public WidgetContext Context { get; set; }
}
