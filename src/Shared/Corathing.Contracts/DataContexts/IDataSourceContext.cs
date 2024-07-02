using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.DataContexts;

public interface IDataSourceContext
{
    void OnCreate(IServiceProvider services, DataSourceState state);
    void Update(DataSourceState state);
    void OnDestroy(DataSourceState state);
}
