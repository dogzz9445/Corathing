using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Configurations;

public class AppPreferenceState
{
    public bool UseSystemTheme { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ApplicationTheme? Theme { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ApplicationLanguage? Language { get; set; } = ApplicationLanguage.en_US;
}
