using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCoraPackageResourceManagerAttribute : Attribute
{
    public Type ResourceManagerParentType { get; }
    public string NameofResourceManager { get; }

    public AssemblyCoraPackageResourceManagerAttribute(Type resourceType, string nameofResourceManager = "ResourceManager")
    {
        ResourceManagerParentType = resourceType;
        NameofResourceManager = nameofResourceManager;
    }
}
