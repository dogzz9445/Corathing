using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Corathing.Organizer.UnitTests.Factories;

public static class ConfigurationFactory
{
    public static IConfigurationRoot Create(string path) =>
         new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();
}
