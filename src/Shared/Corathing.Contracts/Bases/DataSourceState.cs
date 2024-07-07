using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IDataSourceCoreState
{
    string? TypeName { get; }
    string? Title { get; }
}

public class DataSourceCoreState
{
    public string? TypeName { get; set; }
    public string? Title { get; set; }
}

public interface IDataSourceState : IEntity
{
    PackageReferenceState PackageReference { get; }
    DataSourceCoreState CoreSettings { get; }
    object? CustomSettigns { get; }
}

public class DataSourceState : IDataSourceState
{
    public Guid Id { get; set; }
    public PackageReferenceState PackageReference { get; set; }
    public DataSourceCoreState CoreSettings { get; set; }
    public object? CustomSettigns { get; set; }
}
