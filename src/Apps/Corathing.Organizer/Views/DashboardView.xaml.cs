using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.Views;

/// <summary>
/// Represents the type of configuration for a Dashboard. New meaning a new one is being generated, and
/// existing when the dashboard already exists
/// </summary>
public enum DashboardConfigurationType
{
    /// <summary>
    /// New dashboard being generated
    /// </summary>
    New,

    /// <summary>
    /// Existing dashboard being configured
    /// </summary>
    Existing
}
/// <summary>
/// Represents a dashboard model containing widgets and a title
/// Implements the <see cref="Infrastructure.ViewModelBase" />
/// </summary>
/// <seealso cref="Infrastructure.ViewModelBase" />
public partial class DashboardModel : ObservableObject
{
    #region Public Properties
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    [ObservableProperty]
    private string _title;

    /// <summary>
    /// Gets or sets the widgets.
    /// </summary>
    /// <value>The widgets.</value>
    [ObservableProperty]
    private ObservableCollection<WidgetContext> _widgets;

    //private IAppStateService? _appState;

    //[ObservableProperty]
    //private WorkflowState? _workflow;
    //[ObservableProperty]
    //private ObservableCollection<WidgetHost> _widgetHosts;
    //[ObservableProperty]
    //private ObservableCollection<WidgetLayouts> _widgetLayouts;

    //public DashboardHostViewModel()
    //{
    //    WidgetHosts = new ObservableCollection<WidgetHost>();
    //    WidgetLayouts = new ObservableCollection<WidgetLayouts>();
    //}

    //public Task Start(IServiceProvider services, Guid workflowId)
    //{
    //    _appState = services.GetService<IAppStateService>();
    //    if (_appState != null && _appState.TryGetWorkflow(workflowId, out var workflow))
    //    {
    //        Workflow = workflow;
    //    }
    //    else
    //    {
    //        Workflow = null;
    //    }
    //    return Task.CompletedTask;
    //}

    public DashboardModel()
    {
        Widgets = new ObservableCollection<WidgetContext>();
    }

    #endregion Public Properties
}

/// <summary>
/// Provides properties detailing the validity of a dashboard name
/// </summary>
public class DashboardNameValidResponse
{
    #region Public Properties

    /// <summary>
    /// Gets the invalid reason.
    /// </summary>
    /// <value>The invalid reason.</value>
    public string InvalidReason { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="DashboardNameValidResponse"/> is valid.
    /// </summary>
    /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
    public bool Valid { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardNameValidResponse"/> class.
    /// </summary>
    /// <param name="valid">if set to <c>true</c> [valid].</param>
    /// <param name="invalidReason">The invalid reason.</param>
    public DashboardNameValidResponse(bool valid, string invalidReason = null)
    {
        Valid = valid;
        InvalidReason = invalidReason;
    }

    #endregion Public Constructors
}

/// <summary>
/// Interface IDashboardConfigurationHandler
/// </summary>
public interface IDashboardConfigurationHandler
{
    #region Public Methods

    /// <summary>
    /// Complete the dashboard configuration.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="save">if set to <c>true</c> [save].</param>
    /// <param name="newName">The new name.</param>
    void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName);

    /// <summary>
    /// Validate dashboard name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>DashboardNameValidResponse.</returns>
    DashboardNameValidResponse DashboardNameValid(string name);

    #endregion Public Methods
}

/// <summary>
/// View model for dashboards
/// Implements the <see cref="Infrastructure.ViewModelBase" />
/// Implements the <see cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
/// </summary>
/// <seealso cref="Infrastructure.ViewModelBase" />
/// <seealso cref="WpfDashboardControl.Dashboards.IDashboardConfigurationHandler" />
public partial class DashboardsViewModel : ObservableObject, IDashboardConfigurationHandler
{
    #region Private Fields

    //private DashboardSettingsPromptViewModel _configuringDashboard;
    private DashboardModel _selectedDashboard;
    private int _widgetNumber;

    #endregion Private Fields

    #region Public Properties

    //[ObservableProperty]
    //private ObservableCollection<MenuItemViewModel> _addWidgetMenuItemViewModels;

    /// <summary>
    /// Gets the command add widget.
    /// </summary>
    /// <value>The command add widget.</value>
    public ICommand CommandAddWidget => new RelayCommand<WidgetGenerator>(o =>
    {
        var widgetGeneratorToAdd = (WidgetGenerator)o;

        SelectedDashboard.Widgets.Add(widgetGeneratorToAdd.CreateWidget());
        EditMode = true;
    });

    [RelayCommand]
    public void ToggleEditDashboard()
    {
        EditMode = !EditMode;
    }

    /// <summary>
    /// Gets the command configure widget.
    /// </summary>
    /// <value>The command configure widget.</value>
    public ICommand CommandConfigureWidget => new RelayCommand<WidgetHost>(async o =>
    {
        var widgetHost = (WidgetHost)o;
        var parentWindow = Window.GetWindow(widgetHost);
        var window = new BaseWindow();
        if (parentWindow != null)
        {
            window.Owner = parentWindow;
            parentWindow.Effect = new BlurEffect();
            window.CenterWindowToParent();
        }
        var view = new WidgetConfigurationView(widgetHost);
        window.Content = view;
        window.ShowDialog();
        if (parentWindow != null)
        {
            parentWindow.Effect = null;
        }
    });

    /// <summary>
    /// Gets the command done configuring widget.
    /// </summary>
    /// <value>The command done configuring widget.</value>
    //public ICommand CommandDoneConfiguringWidget => new RelayCommand(() => ConfiguringWidget = null);

    /// <summary>
    /// Gets the command edit dashboard.
    /// </summary>
    /// <value>The command edit dashboard.</value>
    //public ICommand CommandEditDashboard => new RelayCommand<string>(o => EditMode = o.ToString() == "True", o => ConfiguringWidget == null);
    public ICommand CommandEditDashboard => new RelayCommand<string>(o => EditMode = o.ToString() == "True", o => true);

    ///// <summary>
    ///// Gets the command manage dashboard.
    ///// </summary>
    ///// <value>The command manage dashboard.</value>
    //public ICommand CommandManageDashboard => new RelayCommand(() =>
    //    ConfiguringDashboard =
    //        new DashboardSettingsPromptViewModel(DashboardConfigurationType.Existing, this,
    //            SelectedDashboard.Title));

    ///// <summary>
    ///// Gets the command new dashboard.
    ///// </summary>
    ///// <value>The command new dashboard.</value>
    //public ICommand CommandNewDashboard => new RelayCommand(() =>
    //    ConfiguringDashboard = new DashboardSettingsPromptViewModel(DashboardConfigurationType.New, this));

    /// <summary>
    /// Gets the command remove widget.
    /// </summary>
    /// <value>The command remove widget.</value>
    public ICommand CommandRemoveWidget => new RelayCommand<WidgetViewModelBase>(o => SelectedDashboard.Widgets.Remove((WidgetViewModelBase)o));

    ///// <summary>
    ///// Gets or sets the configuring dashboard.
    ///// </summary>
    ///// <value>The configuring dashboard.</value>
    //public DashboardSettingsPromptViewModel ConfiguringDashboard
    //{
    //    get => _configuringDashboard;
    //    set => RaiseAndSetIfChanged(ref _configuringDashboard, value);
    //}

    /// <summary>
    /// Gets or sets the dashboards.
    /// </summary>
    /// <value>The dashboards.</value>
    [ObservableProperty]
    private ObservableCollection<DashboardModel>? _dashboards;

    /// <summary>
    /// Gets or sets a value indicating whether [dashboard selector uncheck].
    /// </summary>
    /// <value><c>true</c> if [dashboard selector uncheck]; otherwise, <c>false</c>.</value>
    [ObservableProperty]
    private bool? _dashboardSelectorUncheck;

    /// <summary>
    /// Gets or sets a value indicating whether [edit mode].
    /// </summary>
    /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
    [ObservableProperty]
    private bool? _editMode;

    /// <summary>
    /// Gets or sets the selected dashboard.
    /// </summary>
    /// <value>The selected dashboard.</value>
    public DashboardModel SelectedDashboard
    {
        get => _selectedDashboard;
        set
        {
            if (!SetProperty(ref _selectedDashboard, value))
                return;

            DashboardSelectorUncheck = true;
            DashboardSelectorUncheck = false;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Completes dashboard configuration
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="save">if set to <c>true</c> [save].</param>
    /// <param name="newName">The new name.</param>
    public void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName)
    {
        //ConfiguringDashboard = null;

        if (!save)
            return;

        switch (type)
        {
            case DashboardConfigurationType.New:
                var dashboardModel = new DashboardModel { Title = newName };
                Dashboards.Add(dashboardModel);
                SelectedDashboard = dashboardModel;
                return;
            case DashboardConfigurationType.Existing:
                SelectedDashboard.Title = newName;
                return;
        }
    }

    /// <summary>
    /// Dashboards the name valid.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>DashboardNameValidResponse.</returns>
    public DashboardNameValidResponse DashboardNameValid(string name)
    {
        return Dashboards.Any(dashboard => dashboard.Title == name)
            ? new DashboardNameValidResponse(false, $"That Dashboard Name [{name}] already exists.")
            : new DashboardNameValidResponse(true);
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    /// <returns>Task.</returns>
    public Task Start(IServiceProvider services)
    {
        IAuthService authService = services.GetService<IAuthService>();
        if (authService != null && authService.UseAuthService)
        {
            var loginWindow = new BaseWindow();
            loginWindow.Content = new LoginView();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult == false)
            {
                // System.Shutdown
            }
        }

        // --------------------------------------------------------------------------
        // Available Widgets
        // --------------------------------------------------------------------------

        IPackageService widgetService = services.GetService<IPackageService>();
        widgetService.LoadWidgetsFromDLL("DDT.Core.WidgetSystems.DefaultWidgets.dll");

        // --------------------------------------------------------------------------
        // Load Component Data
        // --------------------------------------------------------------------------
        Dashboards = [new DashboardModel { Title = "My Dashboard" }];
        SelectedDashboard = Dashboards[0];
        AddWidgetMenuItemViewModels = new ObservableCollection<MenuItemViewModel>();

        // --------------------------------------------------------------------------
        // Add Widget Menu
        // --------------------------------------------------------------------------
        AddWidgetMenuItemViewModels.Add(new MenuItemViewModel()
        {
            Header = "Add Widget",
            MenuItems = new ObservableCollection<MenuItemViewModel>(),
        });

        foreach (var widget in widgetService.GetAvailableWidgets())
        {
            var fullMenuHeader = widget.MenuPath;
            if (string.IsNullOrEmpty(fullMenuHeader))
                continue;

            var splitedMenuHeaders = fullMenuHeader.Split('/');
            if (splitedMenuHeaders.Length <= 0)
                continue;

            var parentMenuCollection = AddWidgetMenuItemViewModels.First().MenuItems;

            for (int i = 0; i < splitedMenuHeaders.Length; i++)
            {
                if (i == splitedMenuHeaders.Length - 1)
                {
                    parentMenuCollection.Add(new MenuItemViewModel()
                    {
                        Header = splitedMenuHeaders[i],
                        Command = new RelayCommand(() =>
                        {
                            SelectedDashboard.Widgets.Add(widget.CreateWidget());
                        }, () => true),
                    });
                }
                else
                {
                    var parentMenu = parentMenuCollection.FirstOrDefault(item => item.Header == splitedMenuHeaders[i]);
                    if (parentMenu == null)
                    {
                        parentMenu = new MenuItemViewModel() { Header = splitedMenuHeaders[i] };
                        parentMenu.MenuItems = new ObservableCollection<MenuItemViewModel>();
                        parentMenuCollection.Add(parentMenu);
                    };
                    parentMenuCollection = parentMenu.MenuItems;
                }
            }

        }

        return Task.CompletedTask;
    }

    #endregion Public Methods
}

/// <summary>
/// DashboardView.xaml에 대한 상호 작용 논리
/// </summary>
public partial class DashboardView : System.Windows.Controls.UserControl
{
    public DashboardsViewModel ViewModel;

    public DashboardView()
    {
        InitializeComponent();

        DataContext = ViewModel = new DashboardsViewModel() { EditMode = false };

        Loaded += (s, e) =>
        {
            ViewModel.Start(App.Current.Services);
        };
    }
}
