using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using Corathing.Contracts.Services;

namespace Corathing.Dashboards.WPF.Services;

//public class LocalizationOptions
//{
//    public const string Section = "Corathing:Preferences:Localization";

//    /// <summary>
//    /// The culture to use for localization.
//    /// </summary>
//    [DefaultValue(ApplicationLanguage.ko_KR)]
//    public ApplicationLanguage Culture { get; set; }

//    /// <summary>
//    /// The relative path under application root where resource files are located.
//    /// </summary>
//    public string ResourcesPath { get; set; } = string.Empty;
//}

public class LocalizationService : ILocalizationService, INotifyPropertyChanged
{
    public static new LocalizationService Instance = new();

    private ApplicationLanguage _cachedApplicationLanguage = ApplicationLanguage.Unknown;
    private CultureInfo? _cachedApplicationCultureInfo = null;
    private CultureInfo? _cachedSystemCultureInfo = null;

    private readonly Dictionary<string, ResourceManager> _stringResourceManagers = new Dictionary<string, ResourceManager>();
    private readonly List<Action> _refreshProvideActions = new List<Action>();

    public CultureInfo? CachedSystemCultureInfo
    {
        get => _cachedSystemCultureInfo;
        set => _cachedSystemCultureInfo = value;
    }
    public CultureInfo? CachedApplicationCultureInfo
    {
        get => _cachedApplicationCultureInfo;
        set => _cachedApplicationCultureInfo = value;
    }
    public ApplicationLanguage CachedApplicationLanguage
    {
        get => _cachedApplicationLanguage;
        set
        {
            if (_cachedApplicationLanguage == value)
                return;
            _cachedApplicationLanguage = value;
            RaisePropertyChanged(this, new PropertyChangedEventArgs(null));
        }
    }

    public string? this[string key]
    {
        get => GetString(key);
    }

    public string GetString(string key)
    {
        foreach (var resManager in _stringResourceManagers.Values)
        {
            if (CachedApplicationCultureInfo == null)
            {
                ApplySystemLanguage();
            }
            string? resultString = resManager.GetString(key, CachedApplicationCultureInfo);
            if (!string.IsNullOrEmpty(resultString))
            {
                return resultString;
            }
        }
        return "";
    }

    private bool TryGetString(string key, out string value)
    {
        value = "";
        foreach (var resManager in _stringResourceManagers.Values)
        {
            if (CachedApplicationCultureInfo == null)
            {
                ApplySystemLanguage();
            }
            string? resultString = resManager.GetString(key, CachedApplicationCultureInfo);
            if (!string.IsNullOrEmpty(resultString))
            {
                value = resultString;
                return true;
            }
        }
        return false;
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(sender, e);
        _refreshProvideActions.ForEach(action => action.Invoke());
    }


    public void Apply(ApplicationLanguage language)
    {
        if (CachedApplicationLanguage == language)
            return;

        Apply(ConvertLanguageToCultureInfo(language));
    }

    private void Apply(CultureInfo? culture)
    {
        CachedApplicationCultureInfo = culture;
        CachedApplicationLanguage = ConvertCultureInfoToLanguage(culture);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    public void ApplySystemLanguage()
    {
        Apply(GetSystemCulture());
    }

    public ApplicationLanguage GetAppLanguage()
    {
        if (CachedApplicationLanguage != ApplicationLanguage.Unknown)
            return CachedApplicationLanguage;

        Apply(ApplicationLanguage.en_US);

        var currentCulture = GetAppCulture();
        var currentLanguage = ConvertCultureInfoToLanguage(currentCulture);
        if (CachedApplicationLanguage != currentLanguage)
            CachedApplicationLanguage = currentLanguage;
        return CachedApplicationLanguage;
    }

    public CultureInfo GetAppCulture()
    {
        if (CachedApplicationCultureInfo == null || CachedApplicationCultureInfo != Thread.CurrentThread.CurrentCulture)
            CachedApplicationCultureInfo = Thread.CurrentThread.CurrentCulture;
        return CachedApplicationCultureInfo;
    }

    public CultureInfo GetSystemCulture()
    {
        return GetCachedSystemLanguage();
    }

    private CultureInfo GetCachedSystemLanguage()
    {
        if (CachedSystemCultureInfo != null)
            return CachedSystemCultureInfo;

        UpdateSystemLanguageCache();

        return CachedSystemCultureInfo;
    }

    private void UpdateSystemLanguageCache()
    {
        CachedSystemCultureInfo = GetCurrentSystemLanguage();
    }

    private CultureInfo GetCurrentSystemLanguage()
    {
        return CultureInfo.InstalledUICulture;
    }

    private CultureInfo ConvertLanguageToCultureInfo(ApplicationLanguage language)
        => CultureInfo.GetCultureInfo(ConvertLanuageToString(language));

    private ApplicationLanguage ConvertCultureInfoToLanguage(CultureInfo culture)
        => ConvertStringToLanguage(culture.Name);

    private string ConvertLanuageToString(ApplicationLanguage language)
        => language switch
        {
            ApplicationLanguage.en_US => "en-US",
            ApplicationLanguage.ko_KR => "ko-KR",
            _ => "en-US",
        };

    private ApplicationLanguage ConvertStringToLanguage(string language)
        => language switch
        {
            "en-US" => ApplicationLanguage.en_US,
            "ko-KR" => ApplicationLanguage.ko_KR,
            _ => ApplicationLanguage.en_US,
        };


    /// <summary>
    /// 
    /// </summary>
    /// <param name="namespaceName"></param>
    /// <param name="resourceManager"></param>
    public void RegisterStringResourceManager(string namespaceName, ResourceManager resourceManager)
    {
        _stringResourceManagers.Add(namespaceName, resourceManager);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Provide(string key, Action<string> action, string fallbackValue = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            action?.Invoke(fallbackValue);
            _refreshProvideActions.Add(() => action?.Invoke(fallbackValue));
        }
        else
        {
            action?.Invoke(TryGetString(key, out string value) ? value : fallbackValue);
            _refreshProvideActions.Add(() => action?.Invoke(TryGetString(key, out string value) ? value : fallbackValue));
        }
    }
}
