using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases.Interfaces;

public interface IWidgetLayoutXY
{
    int X { get; }
    int Y { get; }
}

public interface IWidgetLayoutWH
{
    int W { get; }
    int H { get; }
}

public interface IWidgetLayoutRect : IWidgetLayoutXY, IWidgetLayoutWH { }

public interface IWidgetLayout : IEntity
{
    Guid WidgetStateId { get; }
    IWidgetLayoutRect Rect { get; }
    IWidgetLayoutWH MinWH { get; }
}
