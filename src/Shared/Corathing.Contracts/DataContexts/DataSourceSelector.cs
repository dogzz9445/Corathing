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

    private bool _selectDefaultCreateIfEmpty;

    public DataSourceSelector(
        IServiceProvider services,
        Guid? guid = null,
        bool selectDefaultCreateIfEmpty = false)
    {
        _services = services;
        DataSourceContexts = new ObservableCollection<T>();
        HintSelectionText = "Default DataSource";
        _selectDefaultCreateIfEmpty = selectDefaultCreateIfEmpty;

        var dataSourceService = _services.GetService<IDataSourceService>();

        DataSourceContexts = new ObservableCollection<T>(
            dataSourceService.GetAllDataSourceContexts<T>()
        );

        Select(guid);

        WeakReferenceMessenger.Default?.Register<DataSourceStateChangedMessage, string>(
            this,
            typeof(T).FullName,
            OnDataSourceStateChanged);
    }

    [RelayCommand]
    public void OpenDataSourceSettings()
    {
        var navigationService = _services.GetRequiredService<INavigationDialogService>();
        navigationService.NavigateDataSourceSettings(typeof(T), SelectedDataSourceContext).Wait();
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
        if (guid != null)
        {
            SelectedDataSourceContext = DataSourceContexts.FirstOrDefault(c => c.DataSourceId == guid);
        }

        if (SelectedDataSourceContext == null && _selectDefaultCreateIfEmpty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();

            if (DataSourceContexts.Count == 0)
            {
                var dataContext = dataSourceService.CreateDataSourceContext<T>();

                DataSourceContexts.Add(dataContext);
            }
            SelectedDataSourceContext = DataSourceContexts.FirstOrDefault();
        }
    }
}
