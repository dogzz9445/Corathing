using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraPackageAttribute : Attribute
{
    public Type EntryPackageType { get; set; }

    public EntryCoraPackageAttribute(Type entryPackageType)
    {
        EntryPackageType = entryPackageType;
    }
}
