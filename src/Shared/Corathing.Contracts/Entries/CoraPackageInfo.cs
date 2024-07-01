using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

public interface ICoraPackageInfo
{
    List<string> Tags { get; }
}

public class CoraPackageInfo
{
    public List<string> Tags { get; set; }
}
