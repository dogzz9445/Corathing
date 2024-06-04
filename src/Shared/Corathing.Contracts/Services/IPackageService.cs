using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Services;

public interface IPackageService
{
    void RegisterWidgets(List<WidgetGenerator> widgets);
    List<WidgetGenerator> GetAvailableWidgets();
    void LoadWidgetsFromDLL(string pathDLL);
}
