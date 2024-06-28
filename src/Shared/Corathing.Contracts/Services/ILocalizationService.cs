﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Corathing.Contracts.Services;

/// <summary>
/// Unknow can be fallbacked
/// </summary>
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
    string GetString(string key);

    #region Provide Event
    /// <summary>
    /// On LocalizationChanged
    /// </summary>
    /// <param name="action"></param>
    void Provide(string key, Action<string> action, string fallbackValue = "");
    #endregion
}
