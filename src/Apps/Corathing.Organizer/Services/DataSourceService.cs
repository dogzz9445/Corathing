using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;

using NuGet.Packaging;

namespace Corathing.Organizer.Services;

public class DataSourceService(IServiceProvider services) : IDataSourceService
{
    public Dictionary<Guid, DataSourceContext> DataSourceContexts { get; } = new();

    public void AddDataSourceContext(Guid dataSourceStateId, DataSourceContext dataSourceContext)
    {
        DataSourceContexts.Add(dataSourceStateId, dataSourceContext);
    }

    public void AddDataSourceContext(DataSourceContext dataSourceContext)
    {
        DataSourceContexts.Add(dataSourceContext.DataSourceId, dataSourceContext);
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
        DataSourceContexts.Remove(dataSourceContext.DataSourceId);
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
        //return DataSourceContexts.Values.Select(item => item as T).Where(x => x != null);
    }

    public IEnumerable<T> GetDataSourceContexts<T>()
    {
        return DataSourceContexts.Values.OfType<T>();
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
}
