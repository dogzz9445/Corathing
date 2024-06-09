using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Bases;

public class AppPreferences
{
    public ApplicationLanguage? Language { get; set; }
    public ApplicationTheme? Theme { get; set; }
}
