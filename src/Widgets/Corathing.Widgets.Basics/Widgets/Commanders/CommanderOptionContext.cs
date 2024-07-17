using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.Widgets.Commanders;

public class CommanderOptionContext :
    CustomSettingsContext
{
    protected override void OnContextChanged()
    {
        throw new NotImplementedException();
    }

    protected override void OnCreate(object? defaultOption)
    {
        throw new NotImplementedException();
    }

    protected override void OnSettingsChanged(object? option)
    {
        throw new NotImplementedException();
    }
}
