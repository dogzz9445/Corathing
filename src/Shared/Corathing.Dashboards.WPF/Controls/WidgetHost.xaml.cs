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

namespace Corathing.Dashboards.WPF.Controls
{
    /// <summary>
    /// Delegate for creating drag events providing a widgetHost as the parameter
    /// </summary>
    /// <param name="widgetHost">The widget host.</param>
    public delegate void DragEventHandler(WidgetHost widgetHost);

    /// <summary>
    /// Delegate for creating drag events providing a widgetHost as the parameter
    /// </summary>
    /// <param name="widgetHost">The widget host.</param>
    public delegate void MouseEnterEventHandler(WidgetHost widgetHost);

    /// <summary>
    /// Interaction logic for WidgetHost.xaml
    /// </summary>
    public partial class WidgetHost : ContentControl
    {
        #region Private Fields

        // For Resize
        // The part of the rectangle under the mouse.
        public ControlHitType MouseHitType = ControlHitType.None;
        private Point? _mouseDownPoint;
        public Point? MouseDownPoint { get => _mouseDownPoint; }
        private bool? _isInOutlineArea;
        private double _outlineGap = 20;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets the index of the host.
        /// </summary>
        /// <value>The index of the host.</value>
        public Guid Id { get; set; }

        #endregion Public Properties

        #region Public Events

        /// <summary>
        /// Occurs when [drag started].
        /// </summary>
        public event DragEventHandler DragMoveStarted;

        /// <summary>
        /// Occurs when [drag started].
        /// </summary>
        public event DragEventHandler DragResizeStarted;

        /// <summary>
        /// Occurs when [mouse over].
        /// </summary>
        public event MouseEnterEventHandler MouseOver;

        #endregion Public Events

        public WidgetHost()
        {
            InitializeComponent();
            Loaded += WidgetHost_Loaded;
            Unloaded += WidgetHost_Unloaded;
            MouseEnter += (s, e) => MouseOver?.Invoke(this);
        }

        #region Private Methods

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Host_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownPoint = e.GetPosition(this);

            // MyBorder의 위치와 크기를 가져옵니다.
            Rect borderRect = new Rect(new Point(0, 0), this.RenderSize);

            // 외곽선 영역을 정의합니다.
            Rect outerRect = new Rect(borderRect.X, borderRect.Y,
                                      borderRect.Width, borderRect.Height);

            // 내부 영역을 정의합니다.
            Rect innerRect = new Rect(borderRect.X + (_outlineGap / 2), borderRect.Y + (_outlineGap / 2),
                                      borderRect.Width - _outlineGap, borderRect.Height - _outlineGap);

            MouseHitType = SetHitType(outerRect, Mouse.GetPosition(this));
            // 클릭한 위치가 외곽선 영역에 있는지 확인합니다.
            _isInOutlineArea = outerRect.Contains(
                _mouseDownPoint.Value.X,
                _mouseDownPoint.Value.Y)
                && !innerRect.Contains(_mouseDownPoint.Value.X, _mouseDownPoint.Value.Y);

            if (_isInOutlineArea == true)
                DragResizeStarted?.Invoke(this);
        }

        // Return a HitType value to indicate what is at the point.
        private ControlHitType SetHitType(Rect rect, Point point)
        {
            ControlHitType hitType = ControlHitType.Line;
            double left = 0;
            double top = 0;
            double right = rect.Right;
            double bottom = rect.Bottom;
            if (point.X < left) return ControlHitType.None;
            if (point.X > right) return ControlHitType.None;
            if (point.Y < top) return ControlHitType.None;
            if (point.Y > bottom) return ControlHitType.None;

            if (point.X - left < _outlineGap)
                hitType |= ControlHitType.Left;
            else if (right - point.X < _outlineGap)
                hitType |= ControlHitType.Right;
            if (point.Y - top < _outlineGap)
                hitType |= ControlHitType.Top;
            if (bottom - point.Y < _outlineGap)
                hitType |= ControlHitType.Bottom;
            return (hitType & ~ControlHitType.Line) > 0
                ? hitType
                : ControlHitType.Body;
        }

        /// <summary>
        /// Set a mouse cursor appropriate for the current hit type.
        /// </summary>
        /// <returns></returns>
        public Cursor SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case ControlHitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case ControlHitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case ControlHitType.Top | ControlHitType.Left | ControlHitType.Line:
                case ControlHitType.Bottom | ControlHitType.Right | ControlHitType.Line:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case ControlHitType.Top | ControlHitType.Right | ControlHitType.Line:
                case ControlHitType.Bottom | ControlHitType.Left | ControlHitType.Line:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case ControlHitType.Top | ControlHitType.Line:
                case ControlHitType.Bottom | ControlHitType.Line:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case ControlHitType.Left | ControlHitType.Line:
                case ControlHitType.Right | ControlHitType.Line:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor)
            {
                Cursor = desired_cursor;
                Mouse.SetCursor(Cursor);
            }
            return Cursor;
        }

        /// <summary>
        /// Handles the MouseMove event of the Host control. Used to invoke a drag started if the proper
        /// conditions have been met
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void Host_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = e.GetPosition(this);

            // Check if we're "dragging" this control around. If not the return, otherwise invoke DragStarted event.
            if (!(_mouseDownPoint.HasValue) ||
                e.LeftButton == MouseButtonState.Released ||
                Point.Subtract(_mouseDownPoint.Value, mouseMovePoint).Length < SystemParameters.MinimumHorizontalDragDistance &&
                Point.Subtract(_mouseDownPoint.Value, mouseMovePoint).Length < SystemParameters.MinimumVerticalDragDistance)
                return;

            if (_isInOutlineArea == false)
                DragMoveStarted?.Invoke(this);
        }

        /// <summary>
        /// Handles the Loaded event of the WidgetHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void WidgetHost_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WidgetHost_Loaded;
            PreviewMouseLeftButtonDown += Host_MouseLeftButtonDown;
            PreviewMouseMove += Host_PreviewMouseMove;
            MouseLeave += WidgetHost_MouseLeave;
        }

        /// <summary>
        /// Handles the Unloaded event of the WidgetHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void WidgetHost_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= WidgetHost_Unloaded;
            PreviewMouseLeftButtonDown -= Host_MouseLeftButtonDown;
            PreviewMouseMove -= Host_PreviewMouseMove;
            MouseLeave -= WidgetHost_MouseLeave;
        }

        private void WidgetHost_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        #endregion Private Methods
    }
}
