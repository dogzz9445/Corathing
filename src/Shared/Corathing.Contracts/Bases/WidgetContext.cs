using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Contracts.Bases;

public partial class WidgetContext : ObservableRecipient
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private string _widgetTitle;
    [ObservableProperty]
    private bool? _visibleTitle;
    [ObservableProperty]
    private bool? _isSelecting;
    [ObservableProperty]
    private bool? _isDragging;
    [ObservableProperty]
    private bool? _isResizing;
    [ObservableProperty]
    private bool? _isEditing;
    [ObservableProperty]
    private WidgetLayout? _layout;

    public WidgetContext()
    {
        IsSelecting = false;
        IsDragging = false;
        IsResizing = false;
        IsEditing = false;
        VisibleTitle = true;
        WidgetTitle = "Widget";
    }

    public WidgetContext(IServiceProvider services) : this()
    {
        _services = services;
    }
}
