using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CoraDataSourceAttribute : Attribute
{
    public Type DataSourceType { get; }

    public CoraDataSourceAttribute(Type dataSourceType)
    {
        DataSourceType = dataSourceType;
    }
}
