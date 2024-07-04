using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Exetensions;
using Corathing.Contracts.Utils.Helpers;
using Corathing.Dashboards.WPF.Controls;

using Microsoft.Extensions.DependencyInjection;

using Smart.Collections.Generic;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class DataSourceSettingsViewModel : ObservableObject
{
    #region Constructor with IServiceProvider
    private readonly IServiceProvider _services;
    private readonly Guid _id;

    public DataSourceSettingsViewModel(IServiceProvider services)
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

    private Type? _dataSourceType;
    private Type? _optionType;
    private Type? _settingsContextType;

    [ObservableProperty]
    private ObservableCollection<DataSourceContext> _dataSourceContexts;

    [ObservableProperty]
    private DataSourceContext? _selectedContext;

    [ObservableProperty]
    private string _tempName;

    [ObservableProperty]
    private object? _customSettings;

    [ObservableProperty]
    private CustomSettingsContext? _tempSettingsContext;

    public void Initialize(Type? DataSourceType)
    {
        var dataSourceService = _services.GetRequiredService<IDataSourceService>();
        DataSourceContexts = new ObservableCollection<DataSourceContext>(
            dataSourceService.GetAllDataSourceContexts(DataSourceType)
        );
        _dataSourceType = DataSourceType;
    }

    public void LoadDataSourceContexts()
    {
        DataSourceContexts.Clear();
        var dataSourceService = _services.GetRequiredService<IDataSourceService>();
        DataSourceContexts.AddRange(dataSourceService.GetAllDataSourceContexts(_dataSourceType));
    }

    public void Select(DataSourceContext context)
    {
        SelectedContext = context;
    }

    [RelayCommand]
    public void AddDataSource()
    {
        if (_dataSourceType == null)
            return;

        var packageService = _services.GetRequiredService<IPackageService>();
        var dataContext = packageService.CreateDataSourceContext(_dataSourceType.FullName);

        var appStateService = _services.GetRequiredService<IAppStateService>();
        appStateService.UpdateDataSource(dataContext.State);

        var dataSourceService = _services.GetRequiredService<IDataSourceService>();
        dataSourceService.AddDataSourceContext(dataContext);

        DataSourceContexts.Add(dataContext);
        Select(dataContext);
    }

    [RelayCommand]
    public void RemoveDataSource(DataSourceContext context)
    {
        // TODO:
        // 모든 위젯에서 해당 연결 끊기
        //WeakReferenceMessenger.Default.Send(new DataSourceRemovedMessage(context.State.Id));

        var appStateService = _services.GetRequiredService<IAppStateService>();
        var dataSourceService = _services.GetRequiredService<IDataSourceService>();
        appStateService.RemovePackage(context.State.Id);
        dataSourceService.RemoveDataSourceContext(context);
    }

    public void OnSelectedContext(DataSourceContext? selectedContext)
    {
        if (TempSettingsContext != null)
        {
            TempSettingsContext.Destroy();
        }
        SelectedContext = selectedContext;
        if (SelectedContext == null)
        {
            TempName = "";
            TempSettingsContext = null;
            return;
        }

        var packageService = _services.GetRequiredService<IPackageService>();
        _optionType = packageService.GetDataSourceCustomSettingsType(selectedContext.GetType().FullName);
        _settingsContextType = packageService.GetDataSourceSettingsContextType(selectedContext.GetType().FullName);
        if (_optionType != null || _settingsContextType != null)
        {
            TempSettingsContext = packageService.CreateDataSourceSettingsContext(selectedContext.GetType().FullName);
            TempSettingsContext.ApplySettings(JsonHelper.DeepCopy(selectedContext.State.CustomSettigns, _optionType));
        }
    }

    [RelayCommand]
    public void Apply()
    {
        if (SelectedContext == null)
            return;

        // FIXME:
        // 무언가 우아한 방법
        SelectedContext.Name = TempName;
        SelectedContext.State.CoreSettings.Title = TempName;
        SelectedContext.State.CustomSettigns = JsonHelper.DeepCopy(TempSettingsContext.CustomSettings, _optionType);
        SelectedContext.ApplyState(SelectedContext.State);

        var appStateService = _services.GetRequiredService<IAppStateService>();
        appStateService.UpdateDataSource(SelectedContext.State);
    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
    }

    private void OnCustomSettingsChanged(object recipient, CustomSettingsChangedMessage? message)
    {
        CustomSettings = JsonHelper.DeepCopy(message.Value, _optionType);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SelectedContext))
        {
            OnSelectedContext(SelectedContext);
        }
    }
}
