using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Extensions;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Corathing.Dashboards.WPF.Bindings;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using Corathing.Contracts.Utils.Helpers;
using System.Xml.Linq;

namespace Corathing.Dashboards.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DashboardHost.xaml
    /// </summary>
    public partial class DashboardHost : ItemsControl
    {
        #region Public Fields

        public static readonly DependencyProperty ConfigureCommandProperty = DependencyProperty.Register(
            nameof(ConfigureWidgetCommand),
            typeof(ICommand),
            typeof(DashboardHost),
            new PropertyMetadata(default(ICommand)));

        public ICommand ConfigureWidgetCommand
        {
            get => (ICommand)GetValue(ConfigureCommandProperty);
            set => SetValue(ConfigureCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            nameof(RemoveWidgetCommand),
            typeof(ICommand),
            typeof(DashboardHost),
            new PropertyMetadata(default(ICommand)));

        public ICommand RemoveWidgetCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty LayoutChangedCommandProperty = DependencyProperty.Register(
            nameof(LayoutChangedCommand),
            typeof(ICommand),
            typeof(DashboardHost),
            new PropertyMetadata(default));

        public ICommand LayoutChangedCommand
        {
            get => (ICommand)GetValue(LayoutChangedCommandProperty);
            set => SetValue(LayoutChangedCommandProperty, value);
        }

        /// <summary>
        /// The enalbe column limit property
        /// </summary>
        public static readonly DependencyProperty EnableColumnLimitProperty = DependencyProperty.Register(
            nameof(EnableColumnLimit),
            typeof(bool),
            typeof(DashboardHost),
            new PropertyMetadata(true));

        public bool EnableColumnLimit
        {
            get => (bool)GetValue(EnableColumnLimitProperty);
            set => SetValue(EnableColumnLimitProperty, value);
        }

        /// <summary>
        /// The max numbers of columns property
        /// </summary>
        public static readonly DependencyProperty MaxNumColumnsProperty = DependencyProperty.Register(
            nameof(MaxNumColumns),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(32));

        /// <summary>
        /// The number property
        /// </summary>
        public int MaxNumColumns
        {
            get => (int)GetValue(MaxNumColumnsProperty);
            set => SetValue(MaxNumColumnsProperty, value);
        }

        public static readonly DependencyProperty VisibleRowsProperty = DependencyProperty.Register(
            nameof(VisibleRows),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(8));

        public int VisibleRows
        {
            get => (int)GetValue(VisibleRowsProperty);
            set => SetValue(VisibleRowsProperty, value);
        }

        public static readonly DependencyProperty WidgetMinimumHeightProperty = DependencyProperty.Register(
            nameof(WidgetMinimumHeight),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(32));

        public int WidgetMinimumHeight
        {
            get => (int)GetValue(WidgetMinimumHeightProperty);
            set => SetValue(WidgetMinimumHeightProperty, value);
        }

        public static readonly DependencyProperty WidgetMinimumWidthProperty = DependencyProperty.Register(
            nameof(WidgetMinimumWidth),
            typeof(int),
            typeof(DashboardHost),
            new PropertyMetadata(32));

        public int WidgetMinimumWidth
        {
            get => (int)GetValue(WidgetMinimumWidthProperty);
            set => SetValue(WidgetMinimumWidthProperty, value);
        }

        /// <summary>
        /// The edit mode property
        /// </summary>
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(
            nameof(EditMode),
            typeof(bool),
            typeof(DashboardHost),
            new PropertyMetadata(false, (d, e) => ((DashboardHost)d).EditEnabler()));

        /// <summary>
        /// Gets or sets a value indicating whether the dashboard is in [edit mode].
        /// </summary>
        /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty, value);
        }

        public static readonly DependencyProperty CanDropFileProperty = DependencyProperty.Register(
            nameof(CanDropFile),
            typeof(bool),
            typeof(DashboardHost),
            new PropertyMetadata(true));

        public bool CanDropFile
        {
            get => (bool)GetValue(CanDropFileProperty);
            set => SetValue(CanDropFileProperty, value);
        }

        #endregion Public Fields

        #region Private Fields

        // Consts Data
        private const int ScrollIncrement = 15;

        // Dependency Objects
        // ItemsSource Property 에 바인딩된 Notifier
        private PropertyChangeNotifier _itemsSourceChangeNotifier;
        private Border _widgetDestinationHighlight;
        private ScrollViewer _dashboardScrollViewer;

        private readonly List<WidgetHost> _widgetHosts = new List<WidgetHost>();
        private List<WidgetLayout> _widgetLayouts = new List<WidgetLayout>();

        // True if a drag is in progress.
        private bool DragInProgress = false;
        private DragAdorner _draggingAdorner;
        private WidgetHost _draggingHost;
        private WidgetLayout _draggingWidgetLayout;

        private int _hostIndex;

        private Point LastPoint;

        // To change the overall size of the widgets change the value here. This size is considered a block.
        private WidgetLayoutWH _numbersCanEditRowColumn = new WidgetLayoutWH(0, 0);
        private Size _widgetSize = new Size(64, 64);

        private Canvas _widgetsCanvasHost;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Gets the dashboard scroll viewer.
        /// </summary>
        /// <value>The dashboard scroll viewer.</value>
        private ScrollViewer DashboardScrollViewer =>
            _dashboardScrollViewer ??= this.FindChildElementByName<ScrollViewer>("DashboardHostScrollViewer");

        /// <summary>
        /// Gets the widgets canvas host.
        /// </summary>
        /// <value>The widgets canvas host.</value>
        private Canvas WidgetsCanvasHost
        {
            get
            {
                if (_widgetsCanvasHost != null)
                    return _widgetsCanvasHost;

                // We have to **cheat** in order to get the ItemsHost of this ItemsControl by
                // using reflection to gain access to the NonPublic member
                _widgetsCanvasHost = (Canvas)typeof(ItemsControl).InvokeMember("ItemsHost",
                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                    null, this, null);

                WidgetsCanvasHost.HorizontalAlignment = HorizontalAlignment.Left;
                WidgetsCanvasHost.VerticalAlignment = VerticalAlignment.Top;

                SetupCanvases();

                return _widgetsCanvasHost;
            }
        }
        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardHost"/> class.
        /// </summary>
        public DashboardHost()
        {
            InitializeComponent();

            Drop += DashboardHost_Drop;
            ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Canvas)));
            Loaded += DashboardHost_Loaded;
            Unloaded += DashboardHost_Unloaded;

            _itemsSourceChangeNotifier = new PropertyChangeNotifier(this, ItemsSourceProperty);
            _itemsSourceChangeNotifier.ValueChanged += ItemsSource_Changed;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)" /> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (!(element is WidgetHost widgetHost))
                return;

            widgetHost.DragResizeStarted -= WidgetHost_DragResizeStarted;
            widgetHost.DragMoveStarted -= WidgetHost_DragMoveStarted;
            _widgetHosts.Remove(widgetHost);
            _widgetLayouts = _widgetLayouts.Where(widgetData => widgetData.WidgetStateId != widgetHost.Id)
                .ToList();
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WidgetHost { Id = Guid.NewGuid(), EditMode = EditMode };
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="true" /> if the item is (or is eligible to be) its own container; otherwise, <see langword="false" />.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WidgetHost;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (!(element is WidgetHost widgetHost) || WidgetsCanvasHost == null)
                return;

            widgetHost.ConfigureCommand = ConfigureWidgetCommand;
            widgetHost.RemoveCommand = RemoveWidgetCommand;

            var widgetContext = widgetHost.DataContext as WidgetContext;
            widgetContext.EditMode = EditMode;

            Debug.Assert(widgetContext != null, nameof(widgetContext) + " != null");

            // TODO:
            // 여기서 WidgetHost의 Layout을 설정하고, WidgetHost의 위치를 설정해야 함
            // 설정을 읽게 된다면 여기서 WidgetContext 의 정보는 설정으로부터 읽어져옴
            var widgetLayout = widgetContext.Layout;

            Debug.Assert(widgetLayout != null, nameof(widgetLayout) + " != null");

            // 초기 위치
            // 위젯이 탭컨트롤 변경 혹은 설정에 의해서 읽어짐
            // Set min/max dimensions of host so it isn't allowed to grow any larger or smaller
            widgetHost.Id = widgetLayout.WidgetStateId;
            widgetHost.MinHeight = (WidgetMinimumHeight * widgetLayout.H) - widgetHost.Margin.Top - widgetHost.Margin.Bottom;
            widgetHost.MinWidth = (WidgetMinimumWidth * widgetLayout.W) - widgetHost.Margin.Left - widgetHost.Margin.Right;
            widgetHost.Height = (_widgetSize.Height * widgetLayout.H) - widgetHost.Margin.Top - widgetHost.Margin.Bottom;
            widgetHost.Width = (_widgetSize.Width * widgetLayout.W) - widgetHost.Margin.Left - widgetHost.Margin.Right;

            // Subscribe to the widgets drag started and add the widget
            // to the _widgetHosts to keep tabs on it
            widgetHost.DragResizeStarted += WidgetHost_DragResizeStarted;
            widgetHost.DragMoveStarted += WidgetHost_DragMoveStarted;
            _widgetHosts.Add(widgetHost);
            _widgetLayouts.Add(widgetLayout);

            // Check if widget is new by seeing if ColumnIndex or RowIndex are set
            // If it isn't new then just set its location
            var widgetAlreadyThere = WidgetsAtLocation(widgetLayout.WH, widgetLayout.XY).Where(WidgetLayout => WidgetLayout.WidgetStateId != widgetHost.Id);

            if (widgetAlreadyThere == null || !widgetAlreadyThere.Any())
            {
                SetWidgetRowAndColumn(widgetHost, widgetLayout.XY, widgetLayout.WH, false);
                return;
            }

            // widget is new. Find the next available row and column and place the
            // widget then scroll to it if it's offscreen
            var nextAvailable = GetNextAvailableRowColumn(widgetLayout.WH);

            SetWidgetRowAndColumn(widgetHost, nextAvailable, widgetLayout.WH, false);

            // Scroll to the new item if it is off screen
            var widgetsHeight = widgetLayout.H * _widgetSize.Height;
            var widgetEndVerticalLocation = nextAvailable.Y * _widgetSize.Height + widgetsHeight;

            var scrollViewerVerticalScrollPosition =
                DashboardScrollViewer.ViewportHeight + DashboardScrollViewer.VerticalOffset;

            if (!(widgetEndVerticalLocation >= DashboardScrollViewer.VerticalOffset) ||
                !(widgetEndVerticalLocation <= scrollViewerVerticalScrollPosition))
                DashboardScrollViewer.ScrollToVerticalOffset(
                    widgetEndVerticalLocation - widgetsHeight - ScrollIncrement);

            if(LayoutChangedCommand != null && LayoutChangedCommand.CanExecute(null))
            {
                LayoutChangedCommand.Execute(this);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    Trace.WriteLine("Item is removed");

                    if (LayoutChangedCommand != null && LayoutChangedCommand.CanExecute(null))
                    {
                        LayoutChangedCommand.Execute(this);
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the GiveFeedback event of the DraggingHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GiveFeedbackEventArgs"/> instance containing the event data.</param>
        private static void DraggingHost_GiveFeedback(object sender, GiveFeedbackEventArgs args)
        {
            // Due to the DragDrop we have to use the GiveFeedback on the first parameter DependencyObject
            // passed into the DragDrop.DoDragDrop to force the cursor to show SizeAll
            args.UseDefaultCursors = false;
            Mouse.SetCursor(Cursors.SizeAll);
            args.Handled = true;
        }

        /// <summary>
        /// Handles the Loaded event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DashboardHost_Loaded;

            // We only check WidgetsCanvasHost to initialize just in case it wasn't initialized
            // with pre-existing widgets being generated before load
            if (WidgetsCanvasHost == null)
                return;

            SizeChanged += DashboardHost_SizeChanged;
            PreviewDragOver += DashboardHost_PreviewDragOver;
            //DragLeave
        }
        void DashboardHost_PreviewDragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            //double yadjust = myCanvasStretch.Height + e.VerticalChange;
            //double xadjust = myCanvasStretch.Width + e.HorizontalChange;
            //if ((xadjust >= 0) && (yadjust >= 0))
            //{
            //    myCanvasStretch.Width = xadjust;
            //    myCanvasStretch.Height = yadjust;
            //    Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
            //                            e.HorizontalChange);
            //    Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
            //                            e.VerticalChange);
            //    changes.Text = "Size: " +
            //                    myCanvasStretch.Width.ToString() +
            //                     ", " +
            //                    myCanvasStretch.Height.ToString();
            //}
        }
        /// <summary>
        /// Handles the PreviewDragOver event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Only continue if the item being dragged over the DashboardHost is a WidgetHost and
            // the widget host is within the _widgetHosts list
            if (!(e.Data.GetData(typeof(WidgetHost)) is WidgetHost draggingWidgetHost) ||
                _widgetLayouts.FirstOrDefault(widgetLayout => widgetLayout.WidgetStateId == draggingWidgetHost.Id) == null)
                return;

            if (_draggingWidgetLayout == null)
                return;

            // Move the adorner to the appropriate position
            _draggingAdorner.LeftOffset = e.GetPosition(WidgetsCanvasHost).X;
            _draggingAdorner.TopOffset = e.GetPosition(WidgetsCanvasHost).Y;

            var adornerPosition = _draggingAdorner.TransformToVisual(WidgetsCanvasHost).Transform(new Point(0, 0));

            // The adorner will typically start out at X == 0 and Y == 0 which causes an unwanted effect of re-positioning
            // items when it isn't necessary.
            if (adornerPosition.X == 0 && adornerPosition.Y == 0)
                return;

            // When dragging and the adorner gets close to the sides of the scroll viewer then have the scroll viewer
            // automatically scroll in the direction of adorner's edges
            var adornerPositionRelativeToScrollViewer =
                _draggingAdorner.TransformToVisual(DashboardScrollViewer).Transform(new Point(0, 0));

            if (adornerPositionRelativeToScrollViewer.Y + _draggingAdorner.ActualHeight + ScrollIncrement >= DashboardScrollViewer.ViewportHeight)
                DashboardScrollViewer.ScrollToVerticalOffset(DashboardScrollViewer.VerticalOffset + ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.X + _draggingAdorner.ActualWidth + ScrollIncrement >= DashboardScrollViewer.ViewportWidth)
                DashboardScrollViewer.ScrollToHorizontalOffset(DashboardScrollViewer.HorizontalOffset + ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.Y - ScrollIncrement <= 0 && DashboardScrollViewer.VerticalOffset >= ScrollIncrement)
                DashboardScrollViewer.ScrollToVerticalOffset(DashboardScrollViewer.VerticalOffset - ScrollIncrement);
            if (adornerPositionRelativeToScrollViewer.X - ScrollIncrement <= 0 && DashboardScrollViewer.HorizontalOffset >= ScrollIncrement)
                DashboardScrollViewer.ScrollToHorizontalOffset(DashboardScrollViewer.HorizontalOffset - ScrollIncrement);

            // We need to get the adorner imaginary position or position in which we'll use to determine what cell it is hovering over.
            // We do this by getting the width of the host and then divide this by the span + 1
            // In a 1x1 widget this would essentially give us the half way point to which would change the _closestRowColumn
            // In a larger widget (2x2) this would give us the point at 1/3 of the size ensuring the widget can get to its destination more seamlessly
            var addToPositionX = draggingWidgetHost.ActualWidth / (_draggingWidgetLayout.W + 1);
            var addToPositionY = draggingWidgetHost.ActualHeight / (_draggingWidgetLayout.H + 1);

            // Get the closest row/column to the adorner "imaginary" position
            var closestRowColumn =
                GetClosestRowColumn(new Point(adornerPosition.X + addToPositionX, adornerPosition.Y + addToPositionY));

            // Use the canvas to draw a square around the closestRowColumn to indicate where the _draggingWidgetHost will be when mouse is released
            var top = closestRowColumn.Y < 0 ? 0 : closestRowColumn.Y * _widgetSize.Height;
            var left = closestRowColumn.X < 0 ? 0 : closestRowColumn.X * _widgetSize.Width;

            SetElementXY(_widgetDestinationHighlight, left, top);

            // Set the _dragging host into its dragging position
            SetWidgetRowAndColumn(_draggingHost, closestRowColumn, _draggingWidgetLayout.WH);

            // Get all the widgets in the path of where the _dragging host will be set
            var movingWidgets = GetWidgetMoveList(_widgetLayouts.FirstOrDefault(widgetData => widgetData == _draggingWidgetLayout), closestRowColumn, null)
                .OrderBy(widgetData => widgetData.Y)
                .ToList();

            // Move the movingWidgets down in rows the same amount of the _dragging hosts row span
            // unless there is a widget already there in that case increment until there isn't. We
            // used the OrderBy on the movingWidgets to make this work against widgets that have
            // already moved
            var movedWidgets = new List<WidgetLayout>();
            foreach (var widgetLayout in movingWidgets.ToArray())
            {
                // Use the initial amount the dragging widget row size is
                var rowIncrease = 1;

                // Find a row to move it
                while (true)
                {
                    var widgetAtLoc = WidgetsAtLocation(widgetLayout.WH,
                        new WidgetLayoutXY(widgetLayout.X, widgetLayout.Y + rowIncrease))
                        .Where(WidgetLayout => !movingWidgets.Contains(WidgetLayout) || movedWidgets.Contains(WidgetLayout));

                    if (!widgetAtLoc.Any())
                        break;

                    rowIncrease++;
                }

                var movingHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.Id == widgetLayout.WidgetStateId);

                var proposedRow = widgetLayout.Y + rowIncrease;
                for (int row = widgetLayout.RectBeforeDrag.Y; row <= proposedRow; row++)
                {
                    var reArragnedIndex = new WidgetLayoutXY(widgetLayout.X, row);

                    var widgetAlreadyThere = WidgetsAtLocation(widgetLayout.WH, reArragnedIndex)
                        .Where(WidgetLayoutThere => widgetLayout != WidgetLayoutThere && !movingWidgets.Contains(WidgetLayoutThere));

                    if (widgetAlreadyThere.Any())
                        continue;

                    var widgetHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.Id == widgetLayout.WidgetStateId);

                    SetWidgetRowAndColumn(widgetHost, reArragnedIndex, widgetLayout.WH);
                    movingWidgets.Remove(widgetLayout);
                    break;
                }

                movedWidgets.Add(widgetLayout);
            }

            ReArrangeToPreviewLocation();
        }

        /// <summary>
        /// Handles the SizeChanged event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!EditMode)
                return;

            _numbersCanEditRowColumn = new WidgetLayoutWH(0, 0);
            FillCanvasEditingBackground();
        }

        /// <summary>
        /// Handles the Unloaded event of the DashboardHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DashboardHost_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= DashboardHost_Unloaded;
            SizeChanged -= DashboardHost_SizeChanged;
            PreviewDragOver -= DashboardHost_PreviewDragOver;

            if (_itemsSourceChangeNotifier != null)
                _itemsSourceChangeNotifier.ValueChanged -= ItemsSource_Changed;
        }

        /// <summary>
        /// Enabled/Disables edit functionality
        /// </summary>
        private void EditEnabler()
        {
            AllowDrop = EditMode;
            _widgetHosts.ForEach(widgetHost => widgetHost.EditMode = EditMode);

            if (EditMode)
                return;

            // We then need to remove all the extra row and column we no longer need
            _numbersCanEditRowColumn = new WidgetLayoutWH(0, 0);
            RemoveExcessCanvasSize(WidgetsCanvasHost);
        }

        /// <summary>
        /// Fills the canvas editing background.
        /// </summary>
        private void FillCanvasEditingBackground()
        {
            var visibleColumns = EnableColumnLimit ? MaxNumColumns + 1 : GetFullyVisibleColumn() + 1;
            var visibleRows = GetFullyVisibleRow() + 1;

            // Fill Visible Columns
            while (true)
            {
                var columnCount = _numbersCanEditRowColumn.W;

                if (columnCount >= visibleColumns)
                    break;

                _numbersCanEditRowColumn.W += 1;
            }

            // Fill Visible Rows
            //var columnCountForRowAdditions = _numbersCanEditRowColumn.Column;
            while (true)
            {
                var rowCount = _numbersCanEditRowColumn.H;

                if (rowCount >= visibleRows)
                    break;

                _numbersCanEditRowColumn.H += 1;
            }
        }

        private void FixArrangements()
        {
            var arrangementNecessary = true;

            //Need to check for empty spots to see if widgets in rows down from it can possible be placed in those empty spots
            //Once there are no more available to move we set arrangementNecessary to false and we're done
            while (arrangementNecessary)
                arrangementNecessary = ReArrangeFirstEmptySpot();
        }

        /// <summary>
        /// Gets the canvas editing background column count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetCanvasEditingBackgroundColumnCount()
        {
            return _numbersCanEditRowColumn.W;
        }

        /// <summary>
        /// Gets the canvas editing background row count.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetCanvasEditingBackgroundRowCount()
        {
            return _numbersCanEditRowColumn.H;
        }

        /// <summary>
        /// Gets the closest row column from adornerPosition.
        /// </summary>
        /// <param name="adornerPosition">The adorner position.</param>
        /// <returns>RowAndColumn.</returns>
        private WidgetLayoutXY GetClosestRowColumn(Point adornerPosition)
        {
            // First lets get the "real" closest row and column to the adorner Position
            // This is exact location to the square to which the adornerPosition point provided
            // is within
            var realClosestRow = (int)Math.Floor(adornerPosition.Y / _widgetSize.Height);
            var realClosestColumn = (int)Math.Floor(adornerPosition.X / _widgetSize.Width);

            // If the closest row and column are negatives that means the position is off screen
            // at this point we can return 0's and prevent extra calculations by ending this one here
            if (realClosestRow < 0 && realClosestColumn < 0)
                return new WidgetLayoutXY(0, 0);

            // We need to set any negatives to 0 since we can't place anything off screen
            realClosestRow = realClosestRow < 0 ? 0 : realClosestRow;
            realClosestColumn = realClosestColumn < 0 ? 0 : realClosestColumn;
            if (EnableColumnLimit)
            {
                realClosestColumn = (realClosestColumn + _draggingWidgetLayout.W) > MaxNumColumns ? (MaxNumColumns - _draggingWidgetLayout.W) : realClosestColumn;
            }

            var realClosestRowAndColumn = new WidgetLayoutXY(realClosestColumn, realClosestRow);

            //if (_draggingWidgetLayout.WidgetBase.RowIndexColumnIndex.Row == realClosestRow && _draggingWidgetLayout.WidgetBase.RowIndexColumnIndex.Column == realClosestColumn)
            //    return realClosestRowAndColumn;
            return realClosestRowAndColumn;
        }

        /// <summary>
        /// Gets the count of the Columns that are fully visible taking into account
        /// the DashboardScrollViewer could have a horizontal offset
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetFullyVisibleColumn()
        {
            return Convert.ToInt32(Math.Floor(
                (ActualWidth + DashboardScrollViewer.HorizontalOffset) / _widgetSize.Width));
        }

        /// <summary>
        /// Gets the count of the Rows that are fully visible taking into account
        /// the DashboardScrollViewer could have a vertical offset
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int GetFullyVisibleRow()
        {
            return Convert.ToInt32(Math.Floor(
                (ActualHeight + DashboardScrollViewer.VerticalOffset) / _widgetSize.Height));
        }

        /// <summary>
        /// Gets the maximum rows and columns of a grid including their spans as placement.
        /// </summary>
        /// <returns>WpfDashboardControl.Models.RowAndColumn.</returns>
        private WidgetLayoutXY GetMaxRowsAndColumns()
        {
            // Need to get all rows adding their row spans and columns adding their column spans returned back
            // as an array of RowAndColumns
            var widgetsRowsAndColumns = _widgetLayouts
                .Select(widgetData => (widgetData.X + widgetData.W, widgetData.Y + widgetData.H))
                .ToArray();

            if (!widgetsRowsAndColumns.Any())
                return new WidgetLayoutXY(1, 1);

            // Need to get the max row and max columns from the list of RowAndColumns
            var maxColumns = EnableColumnLimit ? MaxNumColumns : widgetsRowsAndColumns
                .Select(rowColumn => rowColumn.Item1)
                .Max();
            var maxRows = widgetsRowsAndColumns
                .Select(rowColumn => rowColumn.Item2)
                .Max();

            return new WidgetLayoutXY(maxColumns, maxRows);
        }

        /// <summary>
        /// Gets the next available row/column.
        /// </summary>
        /// <param name="widgetSpans">The widget spans.</param>
        /// <returns>RowAndColumn.</returns>
        private WidgetLayoutXY GetNextAvailableRowColumn(IWidgetLayoutWH widgetSpans)
        {
            if (_widgetLayouts.Count == 1)
                return new WidgetLayoutXY(0, 0);

            // Get fully visible column count
            var fullyVisibleColumns = GetFullyVisibleColumn();

            // We need to loop through each row and in each row loop through each column
            // to see if the space is currently occupied. When it is available then return
            // what Row and Column the new widget will occupy
            var rowCount = 0;
            while (true)
            {
                for (var column = 0; column < fullyVisibleColumns; column++)
                {
                    var widgetAlreadyThere = WidgetsAtLocation(widgetSpans, new WidgetLayoutXY(column, rowCount));

                    if (widgetAlreadyThere != null && widgetAlreadyThere.Any())
                        continue;

                    // Need to check if the new widget when placed would be outside
                    // the visible columns. If so then we move onto the next row/column
                    var newWidgetSpanOutsideVisibleColumn = false;
                    for (var i = 0; i < widgetSpans.W + 1; i++)
                    {
                        if (column + i <= fullyVisibleColumns)
                            continue;

                        newWidgetSpanOutsideVisibleColumn = true;
                        break;
                    }

                    // The newest widget won't cover up an existing row/column so lets
                    // return the specific row/column the widget can occupy
                    if (!newWidgetSpanOutsideVisibleColumn)
                        return new WidgetLayoutXY(column, rowCount);
                }

                rowCount++;
            }
        }

        /// <summary>
        /// Recursively gets all the widgets in the path of the provided widgetHost into a list.
        /// </summary>
        /// <param name="widgetData">The widget data.</param>
        /// <param name="rowAndColumnPlacement">The row and column placement.</param>
        /// <param name="widgetsThatNeedToMove">The widgets that need to move.</param>
        /// <returns>List&lt;WidgetHost&gt;.</returns>
        private IEnumerable<WidgetLayout> GetWidgetMoveList(WidgetLayout widgetData, WidgetLayoutXY rowAndColumnPlacement, List<WidgetLayout> widgetsThatNeedToMove)
        {
            if (widgetsThatNeedToMove == null)
                widgetsThatNeedToMove = new List<WidgetLayout>();

            var widgetsAtLocation = new List<WidgetLayout>();

            // If the widgetHost is the _draggingHost then we only need to get the direct widgets that occupy the
            // provided rowAndColumnPlacement
            if (widgetData == _draggingWidgetLayout)
            {
                widgetsAtLocation
                    .AddRange(WidgetsAtLocation(widgetData.WH, rowAndColumnPlacement)
                        .Where(widgetAtLocationData => widgetAtLocationData != _draggingWidgetLayout)
                        .ToList());
            }
            else
            {
                // If we're a widget at the designated widgetHost we need to check how many spaces
                // we're moving and check each widget that could potentially be in those spaces
                var widgetRowMovementCount = rowAndColumnPlacement.Y - widgetData.Y + 1;

                for (var i = 0; i < widgetRowMovementCount; i++)
                {
                    widgetsAtLocation
                        .AddRange(WidgetsAtLocation(widgetData.WH, new WidgetLayoutXY(rowAndColumnPlacement.X, widgetData.Y + i))
                        .Where(WidgetLayout => WidgetLayout != widgetData && WidgetLayout != _draggingWidgetLayout));
                }
            }

            // If there aren't any widgets at the location then just return the list we've been maintaining
            if (widgetsAtLocation.Count < 1)
                return widgetsThatNeedToMove.Distinct();

            // Since we have widgets at the designated location we need add to the list any widgets that
            // could potentially move as a result of the widgetHost movement
            for (var widgetAtLocationIndex = 0; widgetAtLocationIndex < widgetsAtLocation.Count; widgetAtLocationIndex++)
            {
                // If we're already tracking the widget then continue to the next
                if (widgetsThatNeedToMove.IndexOf(widgetsAtLocation[widgetAtLocationIndex]) >= 0)
                    continue;

                var widgetDataAtLocation = widgetsAtLocation[widgetAtLocationIndex];

                // Need to recursively check if any widgets that are now in the place that this widget was also get moved down to
                // make room
                var proposedRowAndColumn = new WidgetLayoutXY(widgetDataAtLocation.X, rowAndColumnPlacement.Y + widgetData.H);

                // Get the widgets at the new location this one is moving to
                var currentWidgetsAtNewLocation =
                    WidgetsAtLocation(widgetDataAtLocation.WH, proposedRowAndColumn)
                        .Where(widget =>
                        {
                            if (widget == widgetsAtLocation[widgetAtLocationIndex])
                                return false;

                            var widgetLocationIndex = widgetsAtLocation.IndexOf(widget);

                            // We check here if the potential widget is already scheduled to move and return false in that case
                            return widgetLocationIndex >= 0 && widgetLocationIndex <= widgetAtLocationIndex;
                        })
                        .ToArray();

                // If there are no widgets at the location then we can just add the widget and continue
                if (!currentWidgetsAtNewLocation.Any())
                {
                    widgetsThatNeedToMove.Add(widgetsAtLocation[widgetAtLocationIndex]);
                    GetWidgetMoveList(widgetsAtLocation[widgetAtLocationIndex], proposedRowAndColumn, widgetsThatNeedToMove);
                    continue;
                }

                // We need to get the max row span or size we're dealing with to offset the change
                var maxAdditionalRows = currentWidgetsAtNewLocation
                    .Select(widgetAtNewLocationData => widgetAtNewLocationData.H)
                    .Max();

                // Add the widget to the list and move to the next item
                widgetsThatNeedToMove.Add(widgetsAtLocation[widgetAtLocationIndex]);
                GetWidgetMoveList(widgetsAtLocation[widgetAtLocationIndex],
                    new WidgetLayoutXY(proposedRowAndColumn.X, proposedRowAndColumn.Y + maxAdditionalRows),
                    widgetsThatNeedToMove);
            }

            // Return the list we've been maintaining
            return widgetsThatNeedToMove.Distinct();
        }

        /// <summary>
        /// Handles the Changed event of the ItemsSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void ItemsSource_Changed(object sender, EventArgs args)
        {
            if (ItemsSource == null)
                return;

            // Enforce the ItemsSource be of type ICollect<WidgetContext> as most of the code behind
            // relies on there being a WidgetContext as the item
            if (!(ItemsSource is ICollection<WidgetContext>))
                throw new InvalidOperationException(
                    $"{nameof(DashboardHost)} ItemsSource binding must be an ICollection of {nameof(WidgetContext)} type");
        }

        /// <summary>
        /// Finds the first empty spot and if a widget in a row down from the empty spot can be placed there
        /// it will automatically move that widget there. If it can't it will keep going until it finds a widget
        /// that can move. If it gets to the end and no more widgets can be moved it returns false indicated
        /// there aren't any more moves that can occur
        /// </summary>
        /// <returns><c>true</c> if a widget was moved, <c>false</c> otherwise.</returns>
        private bool ReArrangeFirstEmptySpot()
        {
            // Need to get all rows and columns taking up space
            var maxRowsAndColumn = GetMaxRowsAndColumns();

            // We loop through each row and column on the hunt for a blank space
            for (var mainY = 0; mainY < maxRowsAndColumn.Y; mainY++)
            {
                for (var mainX = 0; mainX < maxRowsAndColumn.X; mainX++)
                {
                    if (WidgetsAtLocation(new WidgetLayoutWH(1, 1), new WidgetLayoutXY(mainX, mainY)).Any())
                        continue;

                    // We need to peak to the next columns to see if they are blank as well
                    var additionalBlankColumnsOnMainRowIndex = 0;
                    for (var subX = mainX + 1; subX < maxRowsAndColumn.X; subX++)
                    {
                        if (WidgetsAtLocation(new WidgetLayoutWH(1, 1),
                            new WidgetLayoutXY(subX, mainY)).Any())
                            break;

                        additionalBlankColumnsOnMainRowIndex++;
                    }

                    var stopChecking = false;

                    // Once we find an empty space we start looping from each row after the mainRowIndex using the same mainColumnIndex + additionalColumnNumber
                    // to find a widget that is a potential candidate to be moved up in rows
                    for (var subY = mainY + 1; subY < maxRowsAndColumn.Y; subY++)
                    {
                        for (var additionalColumnNumber = 0; additionalColumnNumber < additionalBlankColumnsOnMainRowIndex + 1; additionalColumnNumber++)
                        {
                            var secondaryWidgetAtLocation = WidgetsAtLocation(new WidgetLayoutWH(1, 1),
                                new WidgetLayoutXY(mainX + additionalColumnNumber, subY))
                                .ToArray();

                            // We can move on to next row if there is no widget in the space
                            if (!secondaryWidgetAtLocation.Any() || _draggingWidgetLayout != null &&
                                secondaryWidgetAtLocation.First() == _draggingWidgetLayout)
                                continue;

                            var possibleCandidateWidgetData = secondaryWidgetAtLocation.First();

                            // Once we find the next widget in the row we're checking, we need to see if that widget has the same RowIndex and ColumnIndex
                            // of the loops above. If it doesn't we can end looking for a replacement for this spot and find the next empty spot
                            if (possibleCandidateWidgetData.Y != subY ||
                                possibleCandidateWidgetData.X != mainX)
                            {
                                stopChecking = true;
                                break;
                            }

                            // Now we have a good candidate lets see if it'll fit at the location of the empty spot from mainRowIndex and
                            // mainColumnIndex + additionalColumnNumber
                            var mainRowColumnIndex = new WidgetLayoutXY(mainX, mainY);
                            var canSecondaryWidgetBePlacedMainRowColumn =
                                WidgetsAtLocation(possibleCandidateWidgetData.WH, mainRowColumnIndex)
                                    .All(widget => widget == secondaryWidgetAtLocation.First());

                            if (!canSecondaryWidgetBePlacedMainRowColumn)
                            {
                                stopChecking = true;
                                break;
                            }

                            // Everything looks good and the widget can be placed in the empty spot
                            // We also return true here to say that a widget was moved due to this process being ran
                            var movingWidgetHost = _widgetHosts.FirstOrDefault(widgetHost =>
                                widgetHost.Id == possibleCandidateWidgetData.WidgetStateId);

                            SetWidgetRowAndColumn(movingWidgetHost, mainRowColumnIndex, possibleCandidateWidgetData.WH);
                            return true;
                        }

                        if (stopChecking)
                            break;
                    }
                }
            }

            // No more widgets can be moved to fill in empty spots
            return false;
        }

        /// <summary>
        /// Rearrange to preview location
        /// </summary>
        /// <returns><c>true</c> if a widget was moved, <c>false</c> otherwise.</returns>
        private bool ReArrangeToPreviewLocation()
        {
            foreach (var widgetLayout in _widgetLayouts.OrderBy(WidgetLayout => WidgetLayout.H))
            {
                if (widgetLayout == _draggingWidgetLayout)
                    continue;

                for (int row = widgetLayout.RectBeforeDrag.Y; row < widgetLayout.Y; row++)
                {
                    var reArragnedIndex = new WidgetLayoutXY(widgetLayout.X, row);

                    var widgetAlreadyThere = WidgetsAtLocation(widgetLayout.WH, reArragnedIndex)
                        .Where(WidgetLayoutThere => widgetLayout != WidgetLayoutThere);

                    if (widgetAlreadyThere.Any())
                        continue;

                    var widgetHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.Id == widgetLayout.WidgetStateId);

                    SetWidgetRowAndColumn(widgetHost, reArragnedIndex, widgetLayout.WH);
                    widgetLayout.Rect = new WidgetLayoutRect()
                    {
                        X = reArragnedIndex.X,
                        Y = reArragnedIndex.Y,
                        W = widgetLayout.W,
                        H = widgetLayout.H
                    };

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the excess canvas size that is no longer needed.
        /// </summary>
        private void RemoveExcessCanvasSize(Canvas canvas)
        {
            var rowAndColumnMax = GetMaxRowsAndColumns();

            var canvasHasChildren = canvas.Children.Count > 0;
            canvas.Width = (canvasHasChildren ? rowAndColumnMax.X : 0) * _widgetSize.Width;
            canvas.Height = (canvasHasChildren ? rowAndColumnMax.Y : 0) * _widgetSize.Height;
        }

        /// <summary>
        /// Sets up the canvases within the DashboardHost.
        /// </summary>
        private void SetupCanvases()
        {
            // Get the canvas used to show where a dragging widget host will land when dragging ends
            var highlightWidgetCanvas = this.FindChildElementByName<Canvas>("HighlightWidgetCanvas");

            // Add a border control to the canvas which will be manually position manipulated when there is a dragging host
            _widgetDestinationHighlight = new Border
            {
                BorderBrush = Brushes.DeepSkyBlue,
                Background = Brushes.LightBlue,
                Opacity = 0.4,
                BorderThickness = new Thickness(2),
                Visibility = Visibility.Hidden
            };

            highlightWidgetCanvas.Children.Add(_widgetDestinationHighlight);

            WidgetsCanvasHost.Height = 0;
            WidgetsCanvasHost.Width = 0;

            // Add first rectangle for CanvasEditingBackground
            _numbersCanEditRowColumn = new WidgetLayoutWH(1, 1);
        }

        private void AnimateElementXY(
            UIElement uiElement,
            double fromLeft,
            double toLeft,
            double fromTop,
            double toTop,
            int durationFromTo)
        {
            var duration = TimeSpan.FromMilliseconds(200 * (int)Math.Log10(durationFromTo * 10));
            DoubleAnimation animationLeft = new DoubleAnimation(fromLeft, toLeft, duration);
            DoubleAnimation animationTop = new DoubleAnimation(fromTop, toTop, duration);

            uiElement.BeginAnimation(Canvas.LeftProperty, animationLeft);
            uiElement.BeginAnimation(Canvas.TopProperty, animationTop);
        }

        private void SetElementXY(
            FrameworkElement uiElement, double x, double y)
        {
            uiElement.BeginAnimation(Canvas.TopProperty, null);
            uiElement.BeginAnimation(Canvas.LeftProperty, null);

            Canvas.SetTop(uiElement, y);
            Canvas.SetLeft(uiElement, x);
        }

        private void SetElementWH(
            FrameworkElement element, double width, double height)
        {
            if (element == null)
                return;

            element.MaxHeight = height;
            element.MinHeight = height;
            element.Height = height;
            element.Width = width;
            element.MaxWidth = width;
            element.MinWidth = width;
        }

        private void SetElementCellIndexByWidgetSize(
            FrameworkElement uiElement, int columnIndex, int rowIndex)
        {
            if (uiElement == null)
                return;

            columnIndex = columnIndex < 0 ? 0 : columnIndex;
            rowIndex = rowIndex < 0 ? 0 : rowIndex;

            var left = columnIndex * _widgetSize.Width;
            var top = rowIndex * _widgetSize.Height;
            SetElementXY(uiElement, left, top);

            if (uiElement.DataContext is WidgetContext widgetContext)
            {
                widgetContext.State.CoreSettings.ColumnIndex = columnIndex;
                widgetContext.State.CoreSettings.RowIndex = rowIndex;
            }
        }

        private void SetElementCellSpanByWidgetSize(
            FrameworkElement uiElement, int columnSpan, int rowSpan)
        {
            if (uiElement == null)
                return;

            columnSpan = columnSpan < 1 ? 1 : columnSpan;
            rowSpan = rowSpan < 1 ? 1 : rowSpan;

            var width = columnSpan * _widgetSize.Width;
            var height = rowSpan * _widgetSize.Height;
            SetElementWH(uiElement, width, height);

            if (uiElement.DataContext is WidgetContext widgetContext)
            {
                widgetContext.State.CoreSettings.ColumnSpan = columnSpan;
                widgetContext.State.CoreSettings.RowSpan = rowSpan;
            }
        }

        /// <summary>
        /// Sets the widget row and column for the WidgetsCanvasHost and changes the RowIndex and ColumnIndex of
        /// the widgetHost's WidgetBase context.
        /// </summary>
        /// <param name="widgetHost">The widget host.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="rowColumnSpan">The row column span.</param>
        private void SetWidgetRowAndColumn(
            WidgetHost widgetHost,
            IWidgetLayoutXY rowColumnIndex,
            IWidgetLayoutWH rowColumnSpan,
            bool withAnimate = true)
        {
            int rowNumber = rowColumnIndex.Y;
            int columnNumber = rowColumnIndex.X;
            var widgetBase = widgetHost.DataContext as WidgetContext;

            Debug.Assert(widgetBase != null, nameof(widgetBase) + " != null");

            int originalRowNumber = widgetBase.Layout.Y;
            int originalColumnNumber = widgetBase.Layout.X;
            widgetBase.Layout.Rect = new WidgetLayoutRect()
            {
                X = columnNumber,
                Y = rowNumber,
                W = widgetBase.Layout.W,
                H = widgetBase.Layout.H
            };

            var maxRowsAndColumns = GetMaxRowsAndColumns();

            WidgetsCanvasHost.Height = maxRowsAndColumns.Y * _widgetSize.Height;
            WidgetsCanvasHost.Width = maxRowsAndColumns.X * _widgetSize.Width;


            if (withAnimate)
            {
                int distanceFromTo = Math.Abs(rowNumber - originalRowNumber)
                                        + Math.Abs(columnNumber - originalColumnNumber);
                AnimateElementXY(widgetHost,
                    (double)widgetHost.GetValue(Canvas.LeftProperty),
                    columnNumber * _widgetSize.Width,
                    (double)widgetHost.GetValue(Canvas.TopProperty),
                    rowNumber * _widgetSize.Height,
                    distanceFromTo);
            }
            else
            {
                SetElementCellIndexByWidgetSize(widgetHost,
                    columnNumber,
                    rowNumber
                    );
            }

            SetElementCellSpanByWidgetSize(
                widgetHost,
                rowColumnSpan.W,
                rowColumnSpan.H);

            // Update state
            if (widgetHost.DataContext is WidgetContext widgetContext)
            {
                widgetContext.State.CoreSettings.ColumnIndex = columnNumber;
                widgetContext.State.CoreSettings.RowIndex = rowNumber;
            }

            if (!EnableColumnLimit)
            {
                var columnCount = _numbersCanEditRowColumn.W;
                while (true)
                {
                    if (columnCount - 1 >= columnNumber + rowColumnSpan.W - 1)
                        break;
                    _numbersCanEditRowColumn.W += 1;
                }
            }

            while (_numbersCanEditRowColumn.H - 1 < rowNumber + rowColumnSpan.H - 1)
            {
                _numbersCanEditRowColumn.H += 1;
            }
        }

        /// <summary>
        /// Gets a list of WidgetHosts that occupy the provided rowAndColumnToCheck
        /// </summary>
        /// <param name="widgetSpan">The widget span.</param>
        /// <param name="widgetIndex">The row and column to check.</param>
        /// <returns>List<WidgetHost></returns>
        private IEnumerable<WidgetLayout> WidgetsAtLocation(IWidgetLayoutWH widgetSpan, IWidgetLayoutXY widgetIndex)
        {
            // Need to see if a widget is already at the specific row and column
            return _widgetLayouts.Where(widgetData => WidgetLayoutHelper.ItemsCollide(widgetData, widgetIndex, widgetSpan));
        }

        /// <summary>
        /// Handles the DragStarted event of a WidgetHost.
        /// </summary>
        /// <param name="widgetHost">The widget host.</param>
        private void WidgetHost_DragMoveStarted(WidgetHost widgetHost)
        {
            if (!EditMode)
                return;

            if (DragInProgress)
                return;

            try
            {
                // We need to make the DashboardHost allowable to have items dropped on it
                AllowDrop = true;

                _draggingHost = widgetHost;
                _draggingWidgetLayout =
                    _widgetLayouts.FirstOrDefault(widgetData => widgetData.WidgetStateId == _draggingHost.Id);

                _widgetDestinationHighlight.Width =
                    _draggingHost.ActualWidth + _draggingHost.Margin.Left + _draggingHost.Margin.Right;
                _widgetDestinationHighlight.Height =
                    _draggingHost.ActualHeight + _draggingHost.Margin.Top + _draggingHost.Margin.Bottom;
                _widgetDestinationHighlight.Visibility = Visibility.Visible;

                SetElementXY(_widgetDestinationHighlight,
                    _draggingWidgetLayout.X * _widgetSize.Width,
                    _draggingWidgetLayout.Y * _widgetSize.Height);

                // Need to create the adorner that will be used to drag a control around the DashboardHost
                _draggingAdorner = new DragAdorner(_draggingHost, Mouse.GetPosition(_draggingHost));
                _draggingHost.GiveFeedback += DraggingHost_GiveFeedback;
                AdornerLayer.GetAdornerLayer(_draggingHost)?.Add(_draggingAdorner);

                // Need to hide the _draggingHost to give off the illusion that we're moving it somewhere
                _draggingHost.Visibility = Visibility.Hidden;
                _widgetLayouts.ForEach(WidgetLayout =>
                {
                    WidgetLayout.RectBeforeDrag = new WidgetLayoutRect()
                    {
                        X = WidgetLayout.X,
                        Y = WidgetLayout.Y,
                        W = WidgetLayout.W,
                        H = WidgetLayout.H,
                    };
                });

                DragDrop.DoDragDrop(_draggingHost, new DataObject(_draggingHost), DragDropEffects.Move);
            }
            finally
            {
                // Need to cleanup after the DoDragDrop ends by setting back everything to its default state
                _draggingHost.GiveFeedback -= DraggingHost_GiveFeedback;
                Mouse.SetCursor(Cursors.Arrow);
                AllowDrop = false;
                AdornerLayer.GetAdornerLayer(_draggingHost)?.Remove(_draggingAdorner);
                _draggingHost.Visibility = Visibility.Visible;
                _draggingWidgetLayout = null;
                _draggingHost = null;
                _widgetDestinationHighlight.Visibility = Visibility.Hidden;

                if (LayoutChangedCommand != null && LayoutChangedCommand.CanExecute(this))
                {
                    LayoutChangedCommand.Execute(this);
                }
            }
        }

        /// <summary>
        /// Handles the DragStarted event of a WidgetHost.
        /// </summary>
        /// <param name="widgetHost">The widget host.</param>
        private void WidgetHost_DragResizeStarted(WidgetHost widgetHost)
        {
            if (!EditMode || DragInProgress)
                return;

            _draggingHost = widgetHost;
            _draggingWidgetLayout = _widgetLayouts.FirstOrDefault(widgetData => widgetData.WidgetStateId == _draggingHost.Id);

            _widgetDestinationHighlight.Width =
                _draggingHost.ActualWidth + _draggingHost.Margin.Left + _draggingHost.Margin.Right;
            _widgetDestinationHighlight.Height =
                _draggingHost.ActualHeight + _draggingHost.Margin.Top + _draggingHost.Margin.Bottom;
            _widgetDestinationHighlight.Visibility = Visibility.Visible;

            SetElementCellIndexByWidgetSize(
                _widgetDestinationHighlight,
                _draggingWidgetLayout.X,
                _draggingWidgetLayout.Y
                );

            // Need to create the adorner that will be used to drag a control around the DashboardHost
            MouseMove += Dashboard_MouseMove;
            MouseLeftButtonUp += Dashboard_MouseLeftButtonUp;
            MouseLeave += Dashboard_MouseLeave;

            var cursor = _draggingHost.SetMouseCursor();
            Mouse.SetCursor(cursor);
            Cursor = cursor;
            DragInProgress = true;

            // Need to hide the _draggingHost to give off the illusion that we're moving it somewhere
            //_draggingHost.Visibility = Visibility.Hidden;
            _widgetLayouts.ForEach(WidgetLayout =>
            {
                WidgetLayout.RectBeforeDrag = new WidgetLayoutRect()
                {
                    X = WidgetLayout.X,
                    Y = WidgetLayout.Y,
                    W = WidgetLayout.W,
                    H = WidgetLayout.H,
                };
            });
        }

        private void ResizingEnded()
        {
            if (!EditMode || !DragInProgress)
                return;

            DragInProgress = false;

            SetWidgetRowAndColumn(
                _draggingHost,
                _draggingWidgetLayout.XY,
                _draggingWidgetLayout.WH,
                true);

            // Need to cleanup after the DoDragDrop ends by setting back everything to its default state
            MouseLeave -= Dashboard_MouseLeave;
            MouseLeftButtonUp -= Dashboard_MouseLeftButtonUp;
            MouseMove -= Dashboard_MouseMove;
            _draggingHost.Cursor = Cursors.Arrow;
            Cursor = Cursors.Arrow;
            Mouse.SetCursor(Cursors.Arrow);

            _draggingWidgetLayout = null;
            _draggingHost = null;
            _widgetDestinationHighlight.Visibility = Visibility.Hidden;

            if (LayoutChangedCommand != null && LayoutChangedCommand.CanExecute(this))
            {
                LayoutChangedCommand.Execute(this);
            }
        }

        private void Dashboard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResizingEnded();
        }

        private void Dashboard_MouseLeave(object sender, MouseEventArgs e)
        {
            ResizingEnded();
        }

        private void Dashboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress || _draggingHost == null || _draggingWidgetLayout == null)
                return;

            _draggingHost.SetMouseCursor();

            double originalWidth = _draggingWidgetLayout.RectBeforeDrag.W * _widgetSize.Width;
            double originalHeight = _draggingWidgetLayout.RectBeforeDrag.H * _widgetSize.Height;
            double originalLeft = _draggingWidgetLayout.RectBeforeDrag.X * _widgetSize.Width;
            double originalTop = _draggingWidgetLayout.RectBeforeDrag.Y * _widgetSize.Height;

            Point startPoint = new Point(originalLeft, originalTop);
            startPoint.X += _draggingHost.MouseDownPoint.Value.X;
            startPoint.Y += _draggingHost.MouseDownPoint.Value.Y;
            var endPoint = e.GetPosition(this);
            // See how much the mouse has moved.

            // Get the rectangle's current position.
            double new_x = originalLeft;
            double new_y = originalTop;
            double offset_x = 0;
            double offset_y = 0;

            // Update the rectangle.
            if ((_draggingHost.MouseHitType & ControlHitType.Top) > 0)
            {
                offset_y = startPoint.Y - endPoint.Y;
                new_y = originalTop - offset_y;
            }
            if ((_draggingHost.MouseHitType & ControlHitType.Bottom) > 0)
            {
                offset_y = endPoint.Y - startPoint.Y;
            }
            if ((_draggingHost.MouseHitType & ControlHitType.Left) > 0)
            {
                offset_x = startPoint.X - endPoint.X;
                new_x = originalLeft - offset_x;
            }
            if ((_draggingHost.MouseHitType & ControlHitType.Right) > 0)
            {
                offset_x = endPoint.X - startPoint.X;
            }
            double new_width = originalWidth + offset_x;
            double new_height = originalHeight + offset_y;

            var maxRowsAndColumns = GetMaxRowsAndColumns();
            new_width = new_width < _widgetSize.Width ? _widgetSize.Width : new_width;
            new_width = Math.Min(new_width, ((maxRowsAndColumns.X - _draggingWidgetLayout.X) * _widgetSize.Width));
            new_height = new_height < _widgetSize.Height ? _widgetSize.Height : new_height;

            // Don't use negative width or height.
            if (new_width <= 0 && new_height <= 0)
                return;

            SetElementWH(_draggingHost, new_width, new_height);

            int rowIndex = (int)Math.Round(new_y / _widgetSize.Height);
            int columnIndex = (int)Math.Round(new_x / _widgetSize.Width);
            int rowSpan = (int)Math.Round(new_height / _widgetSize.Height);
            int columnSpan = (int)Math.Round(new_width / _widgetSize.Width);

            rowIndex = rowIndex < 0 ? 0 : rowIndex;
            columnIndex = columnIndex < 0 ? 0 : columnIndex;
            rowSpan = rowSpan < 1 ? 1 : rowSpan;
            columnSpan = columnSpan < 1 ? 1 : columnSpan;
            var newRowIndexColumnIndex = new WidgetLayoutXY(columnIndex, rowIndex);
            var newRowSpanColumnSpan = new WidgetLayoutWH(columnSpan, rowSpan);

            // Use the canvas to draw a square around the closestRowColumn to indicate where the _draggingWidgetHost will be when mouse is released
            SetElementCellIndexByWidgetSize(_widgetDestinationHighlight, columnIndex, rowIndex);
            SetElementCellSpanByWidgetSize(_widgetDestinationHighlight, columnSpan, rowSpan);

            _draggingWidgetLayout.Rect = new WidgetLayoutRect()
            {
                X = columnIndex,
                Y = rowIndex,
                W = columnSpan,
                H = rowSpan,
            };

            // Set the _dragging host into its dragging position
            SetElementXY(_draggingHost, new_x, new_y);

            // Get all the widgets in the path of where the _dragging host will be set
            var movingWidgets = GetWidgetMoveList(_widgetLayouts.FirstOrDefault(widgetData => widgetData == _draggingWidgetLayout), newRowIndexColumnIndex, null)
                .OrderBy(widgetData => widgetData.Y)
                .ToList();

            // Move the movingWidgets down in rows the same amount of the _dragging hosts row span
            // unless there is a widget already there in that case increment until there isn't. We
            // used the OrderBy on the movingWidgets to make this work against widgets that have
            // already moved
            var movedWidgets = new List<WidgetLayout>();
            foreach (var widgetData in movingWidgets.ToArray())
            {
                // Use the initial amount the dragging widget row size is
                var rowIncrease = 1;

                // Find a row to move it
                while (true)
                {
                    var widgetAtLoc = WidgetsAtLocation(widgetData.WH,
                        new WidgetLayoutXY(widgetData.X, widgetData.Y + rowIncrease))
                        .Where(WidgetLayout => !movingWidgets.Contains(WidgetLayout) || movedWidgets.Contains(WidgetLayout));

                    if (!widgetAtLoc.Any())
                        break;

                    rowIncrease++;
                }

                var movingHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.Id == widgetData.WidgetStateId);

                var proposedRow = widgetData.Y + rowIncrease;
                for (int row = widgetData.RectBeforeDrag.Y; row <= proposedRow; row++)
                {
                    var reArragnedIndex = new WidgetLayoutXY(widgetData.X, row);

                    var widgetAlreadyThere = WidgetsAtLocation(widgetData.WH, reArragnedIndex)
                        .Where(WidgetLayoutThere => widgetData != WidgetLayoutThere && !movingWidgets.Contains(WidgetLayoutThere));

                    if (widgetAlreadyThere.Any())
                        continue;

                    var widgetHost = _widgetHosts.FirstOrDefault(widgetHost => widgetHost.Id == widgetData.WidgetStateId);

                    SetWidgetRowAndColumn(widgetHost, reArragnedIndex, widgetData.WH);
                    movingWidgets.Remove(widgetData);
                    break;
                }

                movedWidgets.Add(widgetData);
            }

            ReArrangeToPreviewLocation();
        }

        #endregion Private Methods

        private void DashboardHost_Drop(object sender, DragEventArgs e)
        {
            if (CanDropFile && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);
            }
        }
    }
}
