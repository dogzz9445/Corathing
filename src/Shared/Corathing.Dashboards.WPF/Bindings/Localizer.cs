using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Corathing.Dashboards.WPF.Bindings;

public enum AvailableCulture
{
    en_US,
    ko_KR
}

public class LocalizationOptions
{
    public const string Section = "Corathing:Preferences:Localization";

    /// <summary>
    /// The culture to use for localization.
    /// </summary>
    [DefaultValue(AvailableCulture.ko_KR)]
    public AvailableCulture Culture { get; set; }

    /// <summary>
    /// The relative path under application root where resource files are located.
    /// </summary>
    public string ResourcesPath { get; set; } = string.Empty;
}

public class Localizer : INotifyPropertyChanged
{
    #region 1. 싱글톤
    private new static readonly Localizer _instance = new Localizer();
    public static Localizer Instance => _instance;
    #endregion

    public Localizer()
    {
        CurrentCulture = CultureInfo.CurrentCulture;
    }

    //public static void Configure(IConfigurationRoot configuration)
    //{
    //    configuration
    //        .GetSection(LocalizationOptions.Section)
    //        .Bind(Instance._localizationOptions);
    //}

    public const string LocalizationChangedEventMessage = "LocalizationChangeEventMessage";

    private readonly Dictionary<string, ResourceManager> _stringResourceManagers = new Dictionary<string, ResourceManager>();
    private readonly Dictionary<string, ResourceManager> _assetResourceManagers = new Dictionary<string, ResourceManager>();
    private CultureInfo? currentCulture = null;

    public string? this[string key]
    {
        get
        {
            foreach (var resManager in _stringResourceManagers.Values)
            {
                string? resultString = resManager.GetString(key, currentCulture);
                if (!string.IsNullOrEmpty(resultString))
                {
                    return resultString;
                }
            }
            return "";
        }
    }

    public CultureInfo? CurrentCulture
    {
        get { return currentCulture; }
        set
        {
            if (currentCulture != value)
            {
                currentCulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AddStringResourceManager(string baseName, Type type)
    {
        _stringResourceManagers[baseName] = new ResourceManager(type);
    }

    public void AddAssetResourceManager(string baseName, Type type)
    {
        _assetResourceManagers[baseName] = new ResourceManager(type);
    }

    public static string ConvertCultureName(AvailableCulture culture)
        => culture switch
        {
            AvailableCulture.en_US => "en-US",
            AvailableCulture.ko_KR => "ko-KR",
            _ => "ko-KR", // fallback to Korean
        };

    public static void SetCulture(AvailableCulture culture)
        => SetCulture(CultureInfo.GetCultureInfo(ConvertCultureName(culture)));

    public static void SetCulture(CultureInfo? culture)
    {
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Instance.CurrentCulture = culture;
    }

    public static AvailableCulture SetNextAvailableCulture()
    {
        AvailableCulture nextCulture = GetNextAvailableCulture();
        SetCulture(nextCulture);
        return nextCulture;
    }

    public static AvailableCulture GetNextAvailableCulture()
    {
        if (Instance.CurrentCulture == null)
        {
            return AvailableCulture.ko_KR;
        }
        switch (Instance.CurrentCulture.Name)
        {
            case "en-US":
                return AvailableCulture.ko_KR;
            case "ko-KR":
                return AvailableCulture.en_US;
            default:
                return AvailableCulture.ko_KR;
        }
    }

}

public class LocalizationExtension : Binding
{
    public LocalizationExtension(string name) : base("[" + name + "]")
    {
        Mode = BindingMode.OneWay;
        Source = Localizer.Instance;
    }
}
