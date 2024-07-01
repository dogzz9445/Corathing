using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.UnitTests.Factories;

public static class ServiceProviderFactory
{
    public static IServiceProvider Create(string[] args) =>
        new ServiceCollection()
            .BuildServiceProvider();
}
