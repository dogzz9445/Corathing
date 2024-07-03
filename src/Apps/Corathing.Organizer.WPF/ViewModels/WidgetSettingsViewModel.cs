using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Exetensions;
using Corathing.Contracts.Utils.Helpers;
using Corathing.Dashboards.WPF.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class WidgetSettingsViewModel : ObservableObject
{
    #region Constructor with IServiceProvider
    private readonly IServiceProvider _services;
    private Guid _id;

    public WidgetSettingsViewModel(IServiceProvider services)
    {
        _services = services;
        _id = Guid.NewGuid();
        WeakReferenceMessenger.Default.Register<CustomSettingsChangedMessage, Guid>(
            this,
            _id,
            OnCustomSettingsChanged
        );
    }
    #endregion

    // Original properties
    private WidgetHost _originalWidget;
    private WidgetContext _originalContext;

    private WidgetHost _settingsWidgetHost;
    [ObservableProperty]
    private WidgetContext _tempWidgetContext;
    private Type _optionType;
    [ObservableProperty]
    private CustomSettingsContext? _tempCustomSettingsContext;

    public Type RegisterWidget(WidgetHost settingsWidgetHost, WidgetHost originalWidgetHost)
    {
        _originalWidget = originalWidgetHost;
        _settingsWidgetHost = settingsWidgetHost;

        _originalContext = _originalWidget.DataContext as WidgetContext;

        var packageService = _services.GetService<IPackageService>();

        TempWidgetContext = packageService.CreateWidgetContext(_originalContext.GetType().FullName);
        _optionType = packageService.GetWidgetCustomSettingsType(_originalContext.GetType().FullName);
        _originalContext.CopyTo(TempWidgetContext, _optionType);
        _settingsWidgetHost.DataContext = TempWidgetContext;

        // If Custom Settings exists
        // Custom Settings 가 존재할 경우
        if (_optionType != null)
        {
            TempCustomSettingsContext = packageService.CreateWidgetSettingsContext(_originalContext.GetType().FullName);
            TempCustomSettingsContext.RegisterCustomSettings(
                _id,
                JsonHelper.DeepCopy(_originalContext.State.CustomSettings, _optionType)
            );
        }

        return _originalContext.GetType();
    }

    [RelayCommand]
    public void Apply()
    {
        if (_originalContext == null)
            return;

        // FIXME:
        // 무언가 우아한 방법
        TempWidgetContext.CopyToWithoutLayout(_originalContext, _optionType);
        _originalContext.UpdateTo(_originalContext.State);

        var appStateService = _services.GetService<IAppStateService>();
        appStateService.UpdateWidget(_originalContext.State);
    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
    }

    public void OnCustomSettingsChanged(object? sender, CustomSettingsChangedMessage? message)
    {
        TempWidgetContext.State.CustomSettings = JsonHelper.DeepCopy(message.Value, _optionType);
    }
}
