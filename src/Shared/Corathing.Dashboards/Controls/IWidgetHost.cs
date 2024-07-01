using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Dashboards.Controls;

public interface IWidgetHost
{
    public string Title { get; set; }
    public bool EditMode { get; set; }

}
