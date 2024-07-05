using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

public partial class DataSourceSelector<T> :
    ObservableRecipient
    where T : DataSourceContext
{
    private readonly IServiceProvider _services;
    [ObservableProperty]
    private ObservableCollection<T> _dataSourceContexts;

    [ObservableProperty]
    private T? _selectedDataSourceContext;

    [RelayCommand]
    public void OpenDataSourceSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        navigationService.NavigateDataSourceSettings(typeof(T), SelectedDataSourceContext);
    }

    // TODO:
    // Default localizaed Select hint string
    [ObservableProperty]
    private string? _hintSelectionText;

    public DataSourceSelector(
        IServiceProvider services,
        Guid? guid = null)
    {
        _services = services;
        DataSourceContexts = new ObservableCollection<T>();
        HintSelectionText = "Default DataSource";

        var dataSourceService = _services.GetService<IDataSourceService>();

        var dataSources = dataSourceService.GetAllDataSourceContexts<T>().ToList();
        dataSources.ForEach(item => DataSourceContexts.Add(item));

        if (guid != null)
        {
            SelectedDataSourceContext = dataSourceService.GetDataSourceContext<T>(guid);
        }

        WeakReferenceMessenger.Default.Register<DataSourceStateChangedMessage<T>>(this, OnDataSourceStateChanged);
    }

    private void OnDataSourceStateChanged(object recipient, DataSourceStateChangedMessage<T> message)
    {
        if (message.ChangedType == EntityStateChangedType.Added)
        {
            DataSourceContexts.Add(message.Value);
        }
        else if (message.ChangedType == EntityStateChangedType.Removed)
        {
            if (SelectedDataSourceContext == message.Value)
                SelectedDataSourceContext = null;
            DataSourceContexts.Remove(message.Value);
        }
    }

    public void Select(Guid? guid = null)
    {
        SelectedDataSourceContext = guid == null ? null :
            _services.GetRequiredService<IDataSourceService>().GetDataSourceContext<T>(guid);
    }
}

public partial class ExecutableAppDataSourceSelector : DataSourceSelector<ExecutableAppDataSourceContext>
{
    public ExecutableAppDataSourceSelector(IServiceProvider services, Guid? guid = null) : base(services, guid)
    {
    }
}
