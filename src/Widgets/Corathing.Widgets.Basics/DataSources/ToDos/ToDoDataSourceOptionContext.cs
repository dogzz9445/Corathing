using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.ToDos;

public partial class ToDoDataSourceOptionContext
    : CustomSettingsContext
{
    protected override void OnCreate(object? defaultOption)
    {
    }

    protected override void OnContextChanged()
    {
    }

    protected override void OnSettingsChanged(object? option)
    {
    }
}
