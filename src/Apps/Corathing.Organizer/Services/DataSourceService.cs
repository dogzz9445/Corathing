using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

using NuGet.Packaging;

namespace Corathing.Organizer.Services;

public class DataSourceService(IServiceProvider services) : IDataSourceService
{
    public Dictionary<Guid, DataSourceContext> DataSourceContexts { get; } = new();

    public T? GetOrFirstOrCreateDataSourceContext<T>(Guid? guid) where T : DataSourceContext
    {
        var dataContext = GetDataSourceContext<T>(guid);
        if (dataContext != null)
            return dataContext;

        return FirstOrCreateDataSourceContext<T>();
    }

    public T? FirstOrCreateDataSourceContext<T>() where T : DataSourceContext
    {
        var pair = DataSourceContexts.FirstOrDefault();
        if (pair.Value != null)
            return pair.Value as T;

        return CreateDataSourceContext<T>();
    }

    public T? CreateDataSourceContext<T>() where T : DataSourceContext
    {
        return CreateDataSourceContext(typeof(T).FullName) as T;
    }

    public DataSourceContext? CreateDataSourceContext(Type type)
    {
        return CreateDataSourceContext(type.FullName);
    }

    public DataSourceContext? CreateDataSourceContext(string? contextFullName)
    {
        var packageService = services.GetRequiredService<IPackageService>();
        var dataContext = packageService.CreateDataSourceContext(contextFullName);

        var appStateService = services.GetRequiredService<IAppStateService>();
        appStateService.UpdateDataSource(dataContext.State);

        AddDataSourceContext(dataContext);

        WeakReferenceMessenger.Default?.Send(
            new DataSourceStateChangedMessage(EntityStateChangedType.Added, dataContext),
            dataContext.GetType().FullName
            );

        return dataContext;
    }

    public T? GetDataSourceContext<T>(Guid? dataSourceStateId) where T : DataSourceContext
    {
        if (dataSourceStateId == null)
        {
            return null;
        }
        if (DataSourceContexts.TryGetValue(dataSourceStateId.Value, out var dataSourceContext))
        {
            return dataSourceContext as T;
        }
        return null;
    }

    public void DestroyDataSourceContext(DataSourceContext? dataSourceContext)
    {
        // TODO:
        // 모든 위젯에서 해당 연결 끊기
        //WeakReferenceMessenger.Default.Send(new DataSourceRemovedMessage(context.State.Id));

        var appStateService = services.GetRequiredService<IAppStateService>();
        appStateService.RemoveDataSource(dataSourceContext.State.Id);
        RemoveDataSourceContext(dataSourceContext);

        WeakReferenceMessenger.Default?.Send(
            new DataSourceStateChangedMessage(EntityStateChangedType.Removed, dataSourceContext),
            dataSourceContext.GetType().FullName
            );
    }

    public void DestroyDataSourceContext(Guid? guid)
    {
        var dataContext = GetDataSourceContext<DataSourceContext>(guid);

        DestroyDataSourceContext(dataContext);
    }

    public void AddDataSourceContext(Guid dataSourceStateId, DataSourceContext dataSourceContext)
    {
        DataSourceContexts.Add(dataSourceStateId, dataSourceContext);
    }

    public void AddDataSourceContext(DataSourceContext dataSourceContext)
    {
        if (dataSourceContext.DataSourceId == null)
        {
            throw new InvalidOperationException("DataSourceId is null");
        }
        DataSourceContexts.Add(dataSourceContext.DataSourceId.Value, dataSourceContext);
    }

    public void RemoveAllDataSourceContexts<T>()
    {
        var removedList = new List<Guid>();
        removedList.AddRange(DataSourceContexts.Where(x => x.Value is T).Select(pair => pair.Key));
        foreach (var removed in removedList)
        {
            DataSourceContexts.Remove(removed);
        }
    }

    public void RemoveDataSourceContext<T>(T dataSourceContext) where T : DataSourceContext
    {
        if (dataSourceContext.DataSourceId != null)
        {
            DataSourceContexts.Remove(dataSourceContext.DataSourceId.Value);
        }
    }

    public IEnumerable<DataSourceContext> GetAllDataSourceContexts(Type? dataSourceContext)
    {
        if (dataSourceContext == null)
        {
            return DataSourceContexts.Values;
        }
        return DataSourceContexts.Values.Where(x => x.GetType() == dataSourceContext);
    }

    public IEnumerable<T> GetAllDataSourceContexts<T>()
    {
        return DataSourceContexts.Values.OfType<T>();
    }

    public IEnumerable<T> GetDataSourceContexts<T>()
    {
        return DataSourceContexts.Values.OfType<T>();
    }
}
