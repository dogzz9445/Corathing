using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.DataContexts;

public interface IDataSourceCustomSettingsContext
{
    object? CustomSettings { get; set; }
    void UpdateSettings();
}
