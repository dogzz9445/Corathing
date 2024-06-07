using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public class AppSettings
{
    public bool? UseGlobalConfiguration { get; set; }
    public bool? UseAppPathConfiguration { get; set; }
    public string? CustomPath { get; set; }
}
