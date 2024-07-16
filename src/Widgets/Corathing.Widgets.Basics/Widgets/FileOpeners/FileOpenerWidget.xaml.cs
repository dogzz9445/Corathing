using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.CodeAnalysis;
using Wpf.Ui.Controls;

using static System.Windows.Forms.AxHost;

using DataFormats = System.Windows.DataFormats;
using UserControl = System.Windows.Controls.UserControl;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

/// <summary>
/// Interaction logic for FileOpenerWidget.xaml
/// </summary>
public partial class FileOpenerWidget : UserControl
{
    /// <summary>Identifies the <see cref="Symbol"/> dependency property.</summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol),
        typeof(SymbolRegular),
        typeof(FileOpenerWidget),
        new PropertyMetadata(SymbolRegular.Empty)
    );

    /// <summary>
    /// Gets or sets displayed <see cref="SymbolRegular"/>.
    /// </summary>
    public SymbolRegular Symbol
    {
        get => (SymbolRegular)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// The edit mode property
    /// </summary>
    public static readonly DependencyProperty IsDraggingOverProperty = DependencyProperty.Register(
        nameof(IsDraggingOver),
        typeof(bool),
        typeof(FileOpenerWidget),
        new PropertyMetadata(false, (d, e) => ((FileOpenerWidget)d).DragOverEnabler()));

    /// <summary>
    /// Gets or sets a value indicating whether the dashboard is in [edit mode].
    /// </summary>
    /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
    public bool IsDraggingOver
    {
        get => (bool)GetValue(IsDraggingOverProperty);
        set => SetValue(IsDraggingOverProperty, value);
    }

    bool _isDraggingLeaveProgress = false;

    public FileOpenerWidget()
    {
        InitializeComponent();
    }

    public void DragOverEnabler()
    {

    }

    private void Button_FileOrFolder_PreviewDrop(object sender, System.Windows.DragEventArgs e)
    {
        if (DataContext is not FileOpenerWidgetContext context)
            return;

        _isDraggingLeaveProgress = false;
        IsDraggingOver = false;

        context.OnDrop(e);
    }


    private void Button_FileOrFolder_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
    {
        _isDraggingLeaveProgress = false;
        if (e.Data == null)
            return;

        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        if (e.Data.GetData(DataFormats.FileDrop) == null)
            return;

        IsDraggingOver = true;
    }

    private void Button_FileOrFolder_PreviewDragLeave(object sender, System.Windows.DragEventArgs e)
    {
        _isDraggingLeaveProgress = true;
        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (_isDraggingLeaveProgress) IsDraggingOver = false;
        }));
    }

    private void Button_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
        _isDraggingLeaveProgress = false;
    }
}
