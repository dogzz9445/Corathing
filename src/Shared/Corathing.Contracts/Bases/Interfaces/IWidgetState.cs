using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases.Interfaces;

public interface IWidgetState<TSettings> : IEntity
{
    string Type { get; }
    IWidgetCoreState CoreState { get; }
    TSettings Settings { get; }
}
