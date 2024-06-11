﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Entries;

namespace Corathing.Contracts.Services;

public interface IPackageService
{
    void RegisterWidgets(List<CoraWidgetGenerator> widgets);
    List<CoraWidgetGenerator> GetAvailableWidgets();
    void LoadWidgetsFromDLL(string pathDLL);
}
