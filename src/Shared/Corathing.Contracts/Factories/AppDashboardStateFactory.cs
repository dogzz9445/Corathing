using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Factories;

public static class AppDashboardStateFactory
{
    public static AppDashboardState Create()
        => new AppDashboardState()
        {
        };
}
