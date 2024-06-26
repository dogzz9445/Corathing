using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;
using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Utils.Helpers;

public static partial class WidgetLayoutHelper
{

    #region 02. Update

    public static List<WidgetLayout> UpdateLayoutItemRect(
        List<WidgetLayout> dashboard,
        Guid itemId,
        WidgetLayoutRect newRect)
    {
        var newDashboard = dashboard.Select(item =>
        {
            if (item.Id == itemId)
                return new WidgetLayout
                {
                    Id = item.Id,
                    WidgetStateId = item.WidgetStateId,
                    Rect = newRect
                };
            else
                return item;
        }).ToList();

        return FixCollisions(newDashboard, itemId);
    }

    public static List<WidgetLayout> CreateDashboard()
    {
        return new List<WidgetLayout>();
    }

    public static (List<WidgetLayout> Dashboard, WidgetLayout LayoutItem) CreateLayoutItems(
        List<WidgetLayout> dashboard,
        Guid id,
        WidgetLayoutRect rect,
        Guid widgetId,
        int maxColumns)
    {
        if (dashboard.Any(item => item.Id == id))
            return (dashboard, null);

        var newItem = new WidgetLayout
        {
            Id = id,
            Rect = FixRect(rect, maxColumns),
            WidgetStateId = widgetId
        };

        var newDashboard = new List<WidgetLayout>(dashboard) { newItem };
        newDashboard = FixCollisions(newDashboard, id);

        return (newDashboard, newItem);
    }

    public static (List<WidgetLayout> dashboard, WidgetLayout LayoutItem) CreateLayoutItemAtFreeArea(
        List<WidgetLayout> dashboard,
        Guid id,
        WidgetLayoutRect size,
        Guid widgetId,
        int maxColumns)
    {
        if (dashboard.Any(item => item.Id == id))
            return (dashboard, null);

        if (size.W > maxColumns)
            return (dashboard, null);

        var sorted = SortItems(dashboard);

        for (int y = 0; ; y++)
        {
            for (int x = 0; x <= maxColumns - size.W;)
            {
                var item = new WidgetLayout
                {
                    Id = id,
                    WidgetStateId = widgetId,
                    Rect = new WidgetLayoutRect { X = x, Y = y, W = size.W, H = size.H }
                };
                var collisions = GetCollisions(sorted, item);
                if (!collisions.Any())
                {
                    var newDashboard = new List<WidgetLayout>(dashboard) { item };
                    return (newDashboard, item);
                }
                else
                {
                    x = collisions[0].Rect.X + collisions[0].Rect.W;
                }
            }
        }
    }

    public static List<WidgetLayout> MoveLayoutItem(
        List<WidgetLayout> dashboard,
        Guid itemId,
        IWidgetLayoutXY toXY,
        int maxColumns)
    {
        var item = dashboard.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return dashboard;

        var newRect = FixRect(new WidgetLayoutRect
        {
            X = toXY.X,
            Y = toXY.Y,
            W = item.Rect.W,
            H = item.Rect.H
        },
        maxColumns);

        if (item.Rect.Y == newRect.Y && item.Rect.X == newRect.X)
            return dashboard;

        return UpdateLayoutItemRect(dashboard, itemId, newRect);
    }

    public static List<WidgetLayout> RemoveLayoutItem(
        List<WidgetLayout> dashboard,
        Guid itemId)
    {
        return dashboard.Where(item => item.Id != itemId).ToList();
    }

    public static List<WidgetLayout> ResizeLayoutItemByEdges(
        List<WidgetLayout> dashboard,
        Guid itemId,
        (int? Left, int? Top, int? Right, int? Bottom) delta,
        (int W, int H) minSize,
        int maxColumns)
    {
        var item = dashboard.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return dashboard;

        var rect = item.Rect;
        int x = rect.X, y = rect.Y, w = rect.W, h = rect.H;

        if (delta.Left.HasValue)
        {
            int deltaLeft = Math.Min(x, Math.Max(minSize.W - w, delta.Left.Value));
            x -= deltaLeft;
            w += deltaLeft;
        }
        if (delta.Top.HasValue)
        {
            int deltaTop = Math.Min(y, Math.Max(minSize.H - h, delta.Top.Value));
            y -= deltaTop;
            h += deltaTop;
        }
        if (delta.Right.HasValue)
        {
            int deltaRight = Math.Min(maxColumns - x - w, Math.Max(minSize.W - w, delta.Right.Value));
            w += deltaRight;
        }
        if (delta.Bottom.HasValue)
        {
            int deltaBottom = Math.Max(minSize.H - h, delta.Bottom.Value);
            h += deltaBottom;
        }

        var newRect = new WidgetLayoutRect { X = x, Y = y, W = w, H = h };

        if (rect.Y == newRect.Y && rect.X == newRect.X && rect.W == newRect.W && rect.H == newRect.H)
            return dashboard;

        return UpdateLayoutItemRect(dashboard, itemId, newRect);
    }

    private static WidgetLayout FindEntityOnList(List<WidgetLayout> dashboard, Guid itemId)
    {
        return dashboard.FirstOrDefault(item => item.Id == itemId);
    }

    private static int FindEntityIndexOnList(List<WidgetLayout> dashboard, Guid itemId)
    {
        return dashboard.FindIndex(item => item.Id == itemId);
    }

    private static List<WidgetLayout> RemoveEntityFromListAtIndex(List<WidgetLayout> dashboard, int index)
    {
        dashboard.RemoveAt(index);
        return dashboard;
    }

    private static List<WidgetLayout> UpdateEntityOnList(List<WidgetLayout> dashboard, WidgetLayout updatedItem)
    {
        return dashboard.Select(item => item.Id == updatedItem.Id ? updatedItem : item).ToList();
    }

    #endregion

    #region 03. Logic

    public static bool ItemsCollide(WidgetLayout item1, IWidgetLayoutXY item2XY, IWidgetLayoutWH item2WH)
    {
        return (
            item1.Rect.X + item1.Rect.W > item2XY.X && item2XY.X + item2WH.W > item1.Rect.X &&
            item1.Rect.Y + item1.Rect.H > item2XY.Y && item2XY.Y + item2WH.H > item1.Rect.Y
        );
    }
    public static bool ItemsCollide(WidgetLayout item1, WidgetLayout item2)
    {
        return (
            item1.Id != item2.Id &&
            item1.Rect.X + item1.Rect.W > item2.Rect.X && item2.Rect.X + item2.Rect.W > item1.Rect.X &&
            item1.Rect.Y + item1.Rect.H > item2.Rect.Y && item2.Rect.Y + item2.Rect.H > item1.Rect.Y
        );
    }


    public static List<T> GetCollisions<T>(List<T> items, WidgetLayout item) where T : WidgetLayout
    {
        return items.Where(i => ItemsCollide(i, item)).ToList();
    }

    public static List<T> SortItems<T>(List<T> items) where T : WidgetLayout
    {
        return items.OrderByDescending(a => a.Rect.Y).ThenByDescending(a => a.Rect.X).ToList();
    }

    public static List<WidgetLayoutMutableRect> FixCollisionsIter(
        List<WidgetLayoutMutableRect> dashboard,
        WidgetLayoutMutableRect item)
    {
        var sorted = SortItems(dashboard);
        var collisions = GetCollisions(sorted, item);

        foreach (var collision in collisions)
        {
            if (collision.Initiator) continue;

            collision.Rect.Y = item.Rect.Y + item.Rect.H;
            dashboard = FixCollisionsIter(dashboard, collision);
        }

        return dashboard;
    }

    public static List<WidgetLayout> FixCollisions(
        List<WidgetLayout> dashboard,
        Guid itemId)
    {
        var layoutMutableRect = dashboard.Select(item => new WidgetLayoutMutableRect
        {
            Id = item.Id,
            WidgetStateId = item.WidgetStateId,
            Rect = new WidgetLayoutRect { X = item.Rect.X, Y = item.Rect.Y, W = item.Rect.W, H = item.Rect.H },
            Initiator = item.Id == itemId
        }).ToList();

        var item = layoutMutableRect.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return dashboard;

        var newDashboard = FixCollisionsIter(layoutMutableRect, item)
            .Select(i => new WidgetLayout
            {
                Id = i.Id,
                WidgetStateId = i.WidgetStateId,
                Rect = i.Rect
            }).ToList();

        return newDashboard;
    }

    public static WidgetLayoutRect FixRect(WidgetLayoutRect rect, int maxColumns)
    {
        int x = Math.Max(0, Math.Min(maxColumns - rect.W, rect.X));
        int y = Math.Max(0, rect.Y);
        int w = Math.Max(0, Math.Min(maxColumns, rect.W));
        int h = Math.Max(0, rect.H);
        return new WidgetLayoutRect { X = x, Y = y, W = w, H = h };
    }

    #endregion
}
