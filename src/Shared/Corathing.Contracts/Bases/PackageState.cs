using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IPackageReferenceState
{
    Guid? PackageId { get; }
    string PackageTitle { get; }
    string AssemblyName { get; }
    string AssemblyVersion { get; }
}

public class PackageReferenceState : IPackageReferenceState
{
    public Guid? PackageId { get; set; }
    public string PackageTitle { get; set; }
    public string AssemblyName { get; set; }
    public string AssemblyVersion { get; set; }
}

public class PackageState : IEntity
{
    public Guid Id { get; set; }
    public string AssemblyName { get; set; }
    public string AssemblyVersion { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
