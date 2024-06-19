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
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.Widgets.Notes;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.Timers;

[EntryCoraWidget(
    viewType: typeof(TimerWidget),
    contextType: typeof(TimerWidgetViewModel),
    dataTemplateSource: "Widgets/Timers/DataTemplates.xaml",
    name: "Create Timer",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Timer",
    menuOrder: 0
    )]
public partial class TimerWidgetViewModel : WidgetContext
{
    #region 01. Timer
    private DispatcherTimer? _timer;

    [ObservableProperty]
    private TimeSpan _configuringTime;
    [ObservableProperty]
    private TimeSpan _ramainingTime;
    [ObservableProperty]
    private bool _isRunning;
    #endregion

    #region 02. Progressbar

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public TimerWidgetViewModel(IServiceProvider services) : base(services)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide(
            "Corathing.Widgets.Basics.TimerName",
            value => WidgetTitle = value,
            fallbackValue: "Timer");

        IsRunning = false;
        ConfiguringTime = TimeSpan.FromSeconds(10);
        RamainingTime = TimeSpan.FromSeconds(10);

        _timer = new DispatcherTimer();
        _timer.Tick += OnTimerTick;
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _timer?.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        RamainingTime -= TimeSpan.FromSeconds(1);
    }

    [RelayCommand]
    public void ToggleTimer()
    {
        if (EditMode == true)
            return;

        if (IsRunning)
            Stop();
        else
            Start();
    }

    [RelayCommand]
    public void Start()
    {
        if (EditMode == true)
            return;

        IsRunning = true;
        _timer.Start();
    }

    [RelayCommand]
    public void Stop()
    {
        if (EditMode == true)
            return;

        IsRunning = false;
        _timer.Stop();
    }

    [RelayCommand]
    public void Reset()
    {
        if (EditMode == true)
            return;

        RamainingTime = ConfiguringTime;
    }
}

/// <summary>
/// Interaction logic for TimerWidget.xaml
/// </summary>
public partial class TimerWidget
{
    public TimerWidget()
    {
        InitializeComponent();
    }
}
