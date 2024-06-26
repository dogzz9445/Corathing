﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Configurations;

public class AppSettings
{
    public bool? UseGlobalConfiguration { get; set; }
    public bool? UseAppPathConfiguration { get; set; }
    public bool? UseCustomConfiguration { get; set; }
    public string? CustomConfigurationFilename { get; set; }
}
