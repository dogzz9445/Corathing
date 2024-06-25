using System;
using System.Collections.Generic;
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

using ControlzEx.Standard;

using Corathing.Organizer.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Smart.Navigation;

using Wpf.Ui;
using Wpf.Ui.Controls;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

using Size = System.Windows.Size;
using UserControl = System.Windows.Forms.UserControl;

namespace Corathing.Organizer.Views;

/// <summary>
/// Interaction logic for NavigationDialogView.xaml
/// </summary>
public partial class NavigationDialogView
{

    /// <summary>Identifies the <see cref="Closed"/> routed event.</summary>
    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
        nameof(Closed),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<NavigationDialogView, ContentDialogClosedEventArgs>),
        typeof(NavigationDialogView)
    );

    /// <summary>
    /// Occurs after the dialog is closed.
    /// </summary>
    public event TypedEventHandler<NavigationDialogView, ContentDialogClosedEventArgs> Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }


    public NavigationDialogViewModel ViewModel { get; set; }

    public ContentPresenter? DialogHost { get; set; }
    protected TaskCompletionSource<ContentDialogResult>? Tcs { get; set; }
    public Navigator Navigator { get; set; }

    public NavigationDialogView()
    {
        InitializeComponent();

        ViewModel = App.Current.Services.GetService<NavigationDialogViewModel>();
        DataContext = this;

        Navigator = new NavigatorConfig()
            .UseWindowsNavigationProvider()
            .ToNavigator();
    }

    public void Show()
    {
        Visibility = Visibility.Visible;
    }
    public void Hide()
    {
        Visibility = Visibility.Collapsed;
    }

    public void Navigate(FrameworkElement view)
    {
        Navigator.Push(view);
    }

    public void GoBack()
    {
        Navigator.Pop();
    }

    /// <summary>
    /// Occurs after ContentPresenter.Content = null
    /// </summary>
    protected virtual void OnClosed(ContentDialogResult result)
    {
        var closedEventArgs = new ContentDialogClosedEventArgs(ClosedEvent, this) { Result = result };

        RaiseEvent(closedEventArgs);
    }

    private void NavigationView_Navigating(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatingCancelEventArgs args)
    {
    }

    private void NavigationView_Navigated(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatedEventArgs args)
    {
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not Wpf.Ui.Controls.NavigationView navigationView)
        {
            return;
        }

        //NavigationView.SetCurrentValue(
        //    NavigationView.HeaderVisibilityProperty,
        //    navigationView.SelectedItem?.TargetPageType != typeof(DashboardPage)
        //        ? Visibility.Visible
        //        : Visibility.Collapsed
        //);
    }

    private void OnNavigating(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatingCancelEventArgs args)
    {
    }

    #region Size

    /// <summary>Identifies the <see cref="DialogWidth"/> dependency property.</summary>
    public static readonly DependencyProperty DialogWidthProperty = DependencyProperty.Register(
        nameof(DialogWidth),
        typeof(double),
        typeof(NavigationDialogView),
        new PropertyMetadata(double.PositiveInfinity)
    );

    /// <summary>Identifies the <see cref="DialogHeight"/> dependency property.</summary>
    public static readonly DependencyProperty DialogHeightProperty = DependencyProperty.Register(
        nameof(DialogHeight),
        typeof(double),
        typeof(NavigationDialogView),
        new PropertyMetadata(double.PositiveInfinity)
    );

    /// <summary>Identifies the <see cref="DialogMaxWidth"/> dependency property.</summary>
    public static readonly DependencyProperty DialogMaxWidthProperty = DependencyProperty.Register(
        nameof(DialogMaxWidth),
        typeof(double),
        typeof(NavigationDialogView),
        new PropertyMetadata(double.PositiveInfinity)
    );

    /// <summary>Identifies the <see cref="DialogMaxHeight"/> dependency property.</summary>
    public static readonly DependencyProperty DialogMaxHeightProperty = DependencyProperty.Register(
        nameof(DialogMaxHeight),
        typeof(double),
        typeof(NavigationDialogView),
        new PropertyMetadata(double.PositiveInfinity)
    );

    /// <summary>Identifies the <see cref="DialogMargin"/> dependency property.</summary>
    public static readonly DependencyProperty DialogMarginProperty = DependencyProperty.Register(
        nameof(DialogMargin),
        typeof(Thickness),
        typeof(NavigationDialogView)
    );

    /// <summary>
    /// Gets or sets the width of the <see cref="NavigationDialogView"/>.
    /// </summary>
    public double DialogWidth
    {
        get => (double)GetValue(DialogWidthProperty);
        set => SetValue(DialogWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="NavigationDialogView"/>.
    /// </summary>
    public double DialogHeight
    {
        get => (double)GetValue(DialogHeightProperty);
        set => SetValue(DialogHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the max width of the <see cref="NavigationDialogView"/>.
    /// </summary>
    public double DialogMaxWidth
    {
        get => (double)GetValue(DialogMaxWidthProperty);
        set => SetValue(DialogMaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the max height of the <see cref="NavigationDialogView"/>.
    /// </summary>
    public double DialogMaxHeight
    {
        get => (double)GetValue(DialogMaxHeightProperty);
        set => SetValue(DialogMaxHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin of the <see cref="NavigationDialogView"/>.
    /// </summary>
    public Thickness DialogMargin
    {
        get => (Thickness)GetValue(DialogMarginProperty);
        set => SetValue(DialogMarginProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var rootElement = (UIElement)GetVisualChild(0)!;

        rootElement.Measure(availableSize);
        Size desiredSize = rootElement.DesiredSize;

        Size newSize = GetNewDialogSize(desiredSize);

        SetCurrentValue(DialogHeightProperty, newSize.Height);
        SetCurrentValue(DialogWidthProperty, newSize.Width);

        ResizeWidth(rootElement);
        ResizeHeight(rootElement);

        return desiredSize;
    }

    private Size GetNewDialogSize(Size desiredSize)
    {
        var paddingWidth = Padding.Left + Padding.Right;
        var paddingHeight = Padding.Top + Padding.Bottom;

        var marginHeight = DialogMargin.Bottom + DialogMargin.Top;
        var marginWidth = DialogMargin.Left + DialogMargin.Right;

        var width = desiredSize.Width - marginWidth + paddingWidth;
        var height = desiredSize.Height - marginHeight + paddingHeight;

        return new Size(width, height);
    }

    private void ResizeWidth(UIElement element)
    {
        if (DialogWidth <= DialogMaxWidth)
        {
            return;
        }

        SetCurrentValue(DialogWidthProperty, DialogMaxWidth);
        element.UpdateLayout();

        SetCurrentValue(DialogHeightProperty, element.DesiredSize.Height);

        if (DialogHeight > DialogMaxHeight)
        {
            SetCurrentValue(DialogMaxHeightProperty, DialogHeight);
            /*Debug.WriteLine($"DEBUG | {GetType()} | WARNING | DialogHeight > DialogMaxHeight after resizing width!");*/
        }
    }

    private void ResizeHeight(UIElement element)
    {
        if (DialogHeight <= DialogMaxHeight)
        {
            return;
        }

        SetCurrentValue(DialogHeightProperty, DialogMaxHeight);
        element.UpdateLayout();

        SetCurrentValue(DialogWidthProperty, element.DesiredSize.Width);

        if (DialogWidth > DialogMaxWidth)
        {
            SetCurrentValue(DialogMaxWidthProperty, DialogWidth);
            /*Debug.WriteLine($"DEBUG | {GetType()} | WARNING | DialogWidth > DialogMaxWidth after resizing height!");*/
        }
    }
    #endregion
}
