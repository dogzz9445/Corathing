using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

namespace Corathing.Contracts.Factories;

public static class AppPreferenceStateFactory
{
    public static AppPreferenceState Create()
        => new AppPreferenceState()
        {
            UseSystemTheme = false,
            Theme = ApplicationTheme.Light,
        };
}
