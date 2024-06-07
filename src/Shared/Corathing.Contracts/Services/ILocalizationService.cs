using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Corathing.Contracts.Services;

public enum ApplicationLanguage
{
    Unknown,
    ko_KR,
    en_US,
}

public interface ILocalizationService
{
    void RegisterStringResourceManager(string namespaceName, ResourceManager resourceManager);

    void Apply(ApplicationLanguage language);
    void ApplySystemLanguage();
    ApplicationLanguage GetAppLanguage();
    CultureInfo GetAppCulture();
    CultureInfo GetSystemCulture();
}
