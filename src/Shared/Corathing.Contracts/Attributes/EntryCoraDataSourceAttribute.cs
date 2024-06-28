using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraDataSourceAttribute : Attribute
{
    public Type DataSourceType { get; }
    public EntryCoraDataSourceAttribute(Type dataSourceType)
    {
        DataSourceType = dataSourceType;
    }
}
