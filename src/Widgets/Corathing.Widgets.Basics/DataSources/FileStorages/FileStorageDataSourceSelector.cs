using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.FileStorages;

public partial class FileStorageDataSourceSelector : DataSourceSelector<FileStorageDataSourceContext>
{
    public FileStorageDataSourceSelector(IServiceProvider services, Guid? guid = null) : base(services, guid)
    {
    }
}
