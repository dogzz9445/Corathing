using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Contracts.Services;

public interface IDataSourceService
{
    void AddDataSourceContext(Guid dataSourceStateId, DataSourceContext dataSourceContext);
    void AddDataSourceContext(DataSourceContext dataSourceContext);

    void RemoveAllDataSourceContexts<T>();
    void RemoveDataSourceContext<T>(T dataSourceContext) where T : DataSourceContext;

    IEnumerable<DataSourceContext> GetAllDataSourceContexts(Type? dataSourceContext);

    // Data Sources
    T? GetDataSourceContext<T>(Guid? dataSourceStateId) where T : DataSourceContext;
}
