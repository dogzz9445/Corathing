using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Contracts.Services;

public interface IDataSourceService
{
    T? GetOrFirstOrCreateDataSourceContext<T>(Guid? guid) where T : DataSourceContext;
    T? FirstOrCreateDataSourceContext<T>() where T : DataSourceContext;
    T? CreateDataSourceContext<T>() where T : DataSourceContext;
    DataSourceContext? CreateDataSourceContext(Type type);
    T? GetDataSourceContext<T>(Guid? dataSourceStateId) where T : DataSourceContext;
    void DestroyDataSourceContext(DataSourceContext? dataSourceContext);
    void DestroyDataSourceContext(Guid? guid);

    void AddDataSourceContext(Guid dataSourceStateId, DataSourceContext dataSourceContext);
    void AddDataSourceContext(DataSourceContext dataSourceContext);

    void RemoveAllDataSourceContexts<T>();
    void RemoveDataSourceContext<T>(T dataSourceContext) where T : DataSourceContext;

    IEnumerable<DataSourceContext> GetAllDataSourceContexts(Type? dataSourceContext);
    IEnumerable<T> GetAllDataSourceContexts<T>();
}
