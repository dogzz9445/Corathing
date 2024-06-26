using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCoraPackageDataTemplateAttribute : Attribute
{
    public string DataTemplateSource { get; set; }

    public AssemblyCoraPackageDataTemplateAttribute(string dataTemplateSource)
    {
        DataTemplateSource = dataTemplateSource;
    }
}
