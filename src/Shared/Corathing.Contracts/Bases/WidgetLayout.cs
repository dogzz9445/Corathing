using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public interface IEntityList<T> : IList<T> where T : IEntity { }

public class WidgetLayoutXY : IWidgetLayoutXY
{
    public int X { get; set; }
    public int Y { get; set; }
    public WidgetLayoutXY(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class WidgetLayoutWH : IWidgetLayoutWH
{
    public int W { get; set; }
    public int H { get; set; }
    public WidgetLayoutWH(int w, int h)
    {
        W = w;
        H = h;
    }
}

public class WidgetLayoutRect : IWidgetLayoutRect
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
}

public class WidgetLayout : IWidgetLayout, IWidgetLayoutRect
{
    public Guid Id { get; set; }
    public Guid WidgetStateId { get; set; }

    // FIXME:
    // RectBeforeDrag는 Context 쪽으로 옮기는게 정의상 맞음
    // 임시적인 데이터로 저장되지 않아야함
    public IWidgetLayoutRect RectBeforeDrag { get; set; }
    public IWidgetLayoutRect Rect { get; set; }
    public IWidgetLayoutXY XY => Rect;
    public IWidgetLayoutWH WH => Rect;
    public int X => Rect.X;
    public int Y => Rect.Y;
    public int W => Rect.W;
    public int H => Rect.H;
}

public class Dashboard : List<WidgetLayout>, IEntityList<WidgetLayout> { }

public class WidgetLayoutMutableRect : WidgetLayout
{
    public new WidgetLayoutRect Rect { get; set; }
    public bool Initiator { get; set; }
}
