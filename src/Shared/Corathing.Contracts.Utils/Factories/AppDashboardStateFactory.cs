using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Configurations;

namespace Corathing.Contracts.Utils.Factories;

public static class AppDashboardStateFactory
{
    public static AppDashboardState Create()
        => new AppDashboardState()
        {
            Id = Guid.NewGuid()
        };
}
