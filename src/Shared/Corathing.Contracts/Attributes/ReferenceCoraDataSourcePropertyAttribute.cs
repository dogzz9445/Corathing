using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ReferenceCoraDataSourcePropertyAttribute : Attribute
{
    public Type DataSourceType { get; }

    public ReferenceCoraDataSourcePropertyAttribute(Type dataSourceType)
    {
        DataSourceType = dataSourceType;
    }
}
