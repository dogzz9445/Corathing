using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.DataContexts;

public partial class DataSourceSelector<T> :
    ObservableRecipient
    where T : DataSourceContext
{
    private readonly IServiceProvider _services;
    [ObservableProperty]
    private ObservableCollection<T> _dataSourceContexts;

    [ObservableProperty]
    private T? _selectedDataSourceContext;

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

        DataSourceContexts = new ObservableCollection<T>(
            dataSourceService.GetAllDataSourceContexts<T>()
        );

        if (guid != null)
        {
            SelectedDataSourceContext = dataSourceService.GetDataSourceContext<T>(guid);
        }

        WeakReferenceMessenger.Default.Register<DataSourceStateChangedMessage, string>(
            this,
            typeof(T).FullName,
            OnDataSourceStateChanged);
    }

    [RelayCommand]
    public void OpenDataSourceSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        navigationService.NavigateDataSourceSettings(typeof(T), SelectedDataSourceContext);
    }

    private void OnDataSourceStateChanged(object recipient, DataSourceStateChangedMessage message)
    {
        if (message.ChangedType == EntityStateChangedType.Added)
        {
            if (message.Value is T context)
            {
                DataSourceContexts.Add(message.Value as T);
            }
        }
        else if (message.ChangedType == EntityStateChangedType.Removed)
        {
            if (SelectedDataSourceContext == message.Value)
                SelectedDataSourceContext = null;
            DataSourceContexts.Remove(message.Value as T);
        }
        else if (message.ChangedType == EntityStateChangedType.Selected)
        {
            Select(message.Value.DataSourceId);
        }
    }

    public void Select(Guid? guid)
    {
        SelectedDataSourceContext = DataSourceContexts.FirstOrDefault(c => c.DataSourceId == guid);
    }
}
