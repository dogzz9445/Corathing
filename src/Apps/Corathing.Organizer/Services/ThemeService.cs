using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Corathing.Contracts.Services;
using Microsoft.Win32;

namespace Corathing.Organizer.Services;

public class ThemeInfo
{
    public string? Namespace { get; set; }
    public string? LightUri { get; set; }
    public string? DarkUri { get; set; }
}

public class ThemeService : IThemeService
{
    #region Private Properties
    private readonly IServiceProvider? _serivces;
    private ApplicationTheme _cachedApplicationTheme = ApplicationTheme.Unknown;
    private SystemTheme _cachedSystemTheme = SystemTheme.Unknown;
    public List<ThemeInfo> ThemeInfos = new List<ThemeInfo>();

    /// <summary>
    /// Gets the Windows glass color.
    /// </summary>
    public Color SystemGlassColor => SystemParameters.WindowGlassColor;

    /// <summary>
    /// Gets a value indicating whether the system is currently using the high contrast theme.
    /// </summary>
    public bool SystemHighContrast => SystemParameters.HighContrast;

    /// <summary>
    /// Gets a value that indicates whether the application is currently using the high contrast theme.
    /// </summary>
    /// <returns><see langword="true"/> if application uses high contrast theme.</returns>
    public bool IsHighContrast() => _cachedApplicationTheme == ApplicationTheme.HighContrast;

    /// <summary>
    /// Gets a value that indicates whether the Windows is currently using the high contrast theme.
    /// </summary>
    /// <returns><see langword="true"/> if system uses high contrast theme.</returns>
    public bool IsSystemHighContrast() => SystemHighContrast;

    #endregion



    public ThemeService(IServiceProvider services)
    {
        _serivces = services;
    }

    private Wpf.Ui.Appearance.ApplicationTheme Convert(ApplicationTheme theme)
        => theme switch
        {
            ApplicationTheme.Dark => Wpf.Ui.Appearance.ApplicationTheme.Dark,
            ApplicationTheme.Light => Wpf.Ui.Appearance.ApplicationTheme.Light,
            ApplicationTheme.HighContrast => Wpf.Ui.Appearance.ApplicationTheme.HighContrast,
            _ => Wpf.Ui.Appearance.ApplicationTheme.Unknown
        };

    private Wpf.Ui.Controls.WindowBackdropType Convert(WindowBackdropType theme)
        => theme switch
        {
            WindowBackdropType.None => Wpf.Ui.Controls.WindowBackdropType.None,
            WindowBackdropType.Auto => Wpf.Ui.Controls.WindowBackdropType.Auto,
            WindowBackdropType.Mica => Wpf.Ui.Controls.WindowBackdropType.Mica,
            WindowBackdropType.Acrylic => Wpf.Ui.Controls.WindowBackdropType.Acrylic,
            WindowBackdropType.Tabbed => Wpf.Ui.Controls.WindowBackdropType.Tabbed,
            _ => Wpf.Ui.Controls.WindowBackdropType.None
        };


    public void Register(string stringNamespace, string lightUri, string darkUri)
    {
        if (stringNamespace == null)
            return;

        if (ThemeInfos.Any(e => e.Namespace == stringNamespace))
            return;

        ThemeInfos.Add(new ThemeInfo()
        {
            Namespace = stringNamespace,
            LightUri = lightUri,
            DarkUri = darkUri
        });
    }

    /// <summary>
    /// Changes the current application theme.
    /// </summary>
    /// <param name="applicationTheme">Theme to set.</param>
    /// <param name="backgroundEffect">Whether the custom background effect should be applied.</param>
    /// <param name="updateAccent">Whether the color accents should be changed.</param>
    public void Apply(
        ApplicationTheme applicationTheme,
        WindowBackdropType backgroundEffect = WindowBackdropType.Mica,
        bool updateAccent = true
    )
    {
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Convert(applicationTheme), Convert(backgroundEffect), updateAccent);
        if (updateAccent)
        {
            // FIXME:
            //ApplicationAccentColorManager.Apply(
            //    ApplicationAccentColorManager.GetColorizationColor(),
            //    applicationTheme,
            //    false
            //);
        }

        if (applicationTheme == ApplicationTheme.Unknown)
            return;

        var appDictionaries = App.Current.Resources;
        Collection<ResourceDictionary> applicationDictionaries = appDictionaries.MergedDictionaries;

        foreach (var themeInfo in ThemeInfos)
        {
            var newResourceUri = applicationTheme switch
            {
                ApplicationTheme.Dark => themeInfo.DarkUri,
                ApplicationTheme.Light => themeInfo.LightUri,
                _ => ""
            };
            var oldResourceUri = applicationTheme switch
            {
                ApplicationTheme.Dark => themeInfo.LightUri,
                ApplicationTheme.Light => themeInfo.DarkUri,
                _ => ""
            };
            if (applicationDictionaries.Count == 0 || string.IsNullOrEmpty(newResourceUri))
            {
                continue;
            }

            bool isSourced = false;
            string resourceLookup = "theme";

            for (var i = 0; i < applicationDictionaries.Count; i++)
            {
                string sourceUri;

                if (applicationDictionaries[i]?.Source != null)
                {
                    sourceUri = applicationDictionaries[i].Source.ToString().ToLower().Trim();

                    if (sourceUri.Contains(themeInfo.Namespace.ToLower().Trim()) && sourceUri.Contains(resourceLookup))
                    {
                        applicationDictionaries[i] = new() { Source = new Uri(newResourceUri, UriKind.Absolute) };
                        isSourced = true;
                        break;
                    }
                }

                for (var j = 0; j < applicationDictionaries[i].MergedDictionaries.Count; j++)
                {
                    if (applicationDictionaries[i].MergedDictionaries[j]?.Source == null)
                    {
                        continue;
                    }

                    sourceUri = applicationDictionaries[i]
                        .MergedDictionaries[j]
                        .Source.ToString()
                        .ToLower()
                        .Trim();

                    if (!sourceUri.Contains(themeInfo.Namespace.ToLower().Trim()) || !sourceUri.Contains(resourceLookup))
                    {
                        continue;
                    }

                    applicationDictionaries[i].MergedDictionaries[j] = new() { Source = new Uri(newResourceUri, UriKind.Absolute) };
                    isSourced = true;
                }
            }
            if (!isSourced)
                applicationDictionaries.Add(new ResourceDictionary() { Source = new Uri(newResourceUri, UriKind.Absolute) });
        }

        UpdateSystemThemeCache();

        _cachedApplicationTheme = applicationTheme;
    }


    public void ApplySystemTheme()
    {
        ApplySystemTheme(true);
    }

    public void ApplySystemTheme(bool updateAccent)
    {
        UpdateSystemThemeCache();

        SystemTheme systemTheme = GetSystemTheme();

        ApplicationTheme themeToSet = ApplicationTheme.Light;

        if (systemTheme is SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow)
        {
            themeToSet = ApplicationTheme.Dark;
        }
        else if (
            systemTheme is SystemTheme.HC1 or SystemTheme.HC2 or SystemTheme.HCBlack or SystemTheme.HCWhite
        )
        {
            themeToSet = ApplicationTheme.HighContrast;
        }

        Apply(themeToSet, updateAccent: updateAccent);
    }

    /// <summary>
    /// Applies Resources in the <paramref name="frameworkElement"/>.
    /// </summary>
    //public void Apply(FrameworkElement frameworkElement)
    //{
    //    if (frameworkElement is null)
    //    {
    //        return;
    //    }

    //    ResourceDictionary[] resourcesRemove = frameworkElement
    //        .Resources.MergedDictionaries.Where(e => e.Source is not null)
    //        .Where(e => e.Source.ToString().ToLower().Contains(LibraryNamespace))
    //        .ToArray();

    //    foreach (ResourceDictionary? resource in UiApplication.Current.Resources.MergedDictionaries)
    //    {
    //        frameworkElement.Resources.MergedDictionaries.Add(resource);
    //    }

    //    foreach (ResourceDictionary resource in resourcesRemove)
    //    {
    //        _ = frameworkElement.Resources.MergedDictionaries.Remove(resource);
    //    }

    //    foreach (System.Collections.DictionaryEntry resource in UiApplication.Current.Resources)
    //    {
    //        frameworkElement.Resources[resource.Key] = resource.Value;
    //    }
    //}

    /// <summary>
    /// Gets currently set application theme.
    /// </summary>
    /// <returns><see cref="ApplicationTheme.Unknown"/> if something goes wrong.</returns>
    public ApplicationTheme GetAppTheme()
    {
        // FIXME:
        //if (_cachedApplicationTheme == ApplicationTheme.Unknown)
        //{
        //    FetchApplicationTheme();
        //}

        return _cachedApplicationTheme;
    }

    /// <summary>
    /// Gets currently set system theme.
    /// </summary>
    /// <returns><see cref="SystemTheme.Unknown"/> if something goes wrong.</returns>
    public SystemTheme GetSystemTheme()
    {
        return GetCachedSystemTheme();
    }

    /// <summary>
    /// Gets a value that indicates whether the application is matching the system theme.
    /// </summary>
    /// <returns><see langword="true"/> if the application has the same theme as the system.</returns>
    public bool IsAppMatchesSystem()
    {
        ApplicationTheme appApplicationTheme = GetAppTheme();
        SystemTheme sysTheme = GetSystemTheme();

        return appApplicationTheme switch
        {
            ApplicationTheme.Dark
                => sysTheme is SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow,
            ApplicationTheme.Light
                => sysTheme is SystemTheme.Light or SystemTheme.Flow or SystemTheme.Sunrise,
            _ => appApplicationTheme == ApplicationTheme.HighContrast && SystemHighContrast
        };
    }

    /// <summary>
    /// Checks if the application and the operating system are currently working in a dark theme.
    /// </summary>
    public bool IsMatchedDark()
    {
        ApplicationTheme appApplicationTheme = GetAppTheme();
        SystemTheme sysTheme = GetSystemTheme();

        if (appApplicationTheme != ApplicationTheme.Dark)
        {
            return false;
        }

        return sysTheme is SystemTheme.Dark or SystemTheme.CapturedMotion or SystemTheme.Glow;
    }

    /// <summary>
    /// Checks if the application and the operating system are currently working in a light theme.
    /// </summary>
    public bool IsMatchedLight()
    {
        ApplicationTheme appApplicationTheme = GetAppTheme();
        SystemTheme sysTheme = GetSystemTheme();

        if (appApplicationTheme != ApplicationTheme.Light)
        {
            return false;
        }

        return sysTheme is SystemTheme.Light or SystemTheme.Flow or SystemTheme.Sunrise;
    }

    /// <summary>
    /// Tries to guess the currently set application theme.
    /// </summary>
    //private void FetchApplicationTheme()
    //{
    //    var appDictionaries = App.Current.Resources;
    //    ResourceDictionary? themeDictionary = appDictionaries.GetDictionary("theme");

    //    if (themeDictionary == null)
    //    {
    //        return;
    //    }

    //    string themeUri = themeDictionary.Source.ToString().Trim().ToLower();

    //    if (themeUri.Contains("light"))
    //    {
    //        _cachedApplicationTheme = ApplicationTheme.Light;
    //    }

    //    if (themeUri.Contains("dark"))
    //    {
    //        _cachedApplicationTheme = ApplicationTheme.Dark;
    //    }

    //    if (themeUri.Contains("highcontrast"))
    //    {
    //        _cachedApplicationTheme = ApplicationTheme.HighContrast;
    //    }
    //}

    #region System Theme

    /// <summary>
    /// Returns the Windows theme retrieved from the registry. If it has not been cached before, invokes the <see cref="UpdateSystemThemeCache"/> and then returns the currently obtained theme.
    /// </summary>
    /// <returns>Currently cached Windows theme.</returns>
    public SystemTheme GetCachedSystemTheme()
    {
        if (_cachedSystemTheme != SystemTheme.Unknown)
        {
            return _cachedSystemTheme;
        }

        UpdateSystemThemeCache();

        return _cachedSystemTheme;
    }

    /// <summary>
    /// Refreshes the currently saved system theme.
    /// </summary>
    public void UpdateSystemThemeCache()
    {
        _cachedSystemTheme = GetCurrentSystemTheme();
    }

    private SystemTheme GetCurrentSystemTheme()
    {
        var currentTheme =
            Registry.GetValue(
                "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes",
                "CurrentTheme",
                "aero.theme"
            ) as string
            ?? string.Empty;

        if (!string.IsNullOrEmpty(currentTheme))
        {
            currentTheme = currentTheme.ToLower().Trim();

            // This may be changed in the next versions, check the Insider previews
            if (currentTheme.Contains("basic.theme"))
            {
                return SystemTheme.Light;
            }

            if (currentTheme.Contains("aero.theme"))
            {
                return SystemTheme.Light;
            }

            if (currentTheme.Contains("dark.theme"))
            {
                return SystemTheme.Dark;
            }

            if (currentTheme.Contains("hcblack.theme"))
            {
                return SystemTheme.HCBlack;
            }

            if (currentTheme.Contains("hcwhite.theme"))
            {
                return SystemTheme.HCWhite;
            }

            if (currentTheme.Contains("hc1.theme"))
            {
                return SystemTheme.HC1;
            }

            if (currentTheme.Contains("hc2.theme"))
            {
                return SystemTheme.HC2;
            }

            if (currentTheme.Contains("themea.theme"))
            {
                return SystemTheme.Glow;
            }

            if (currentTheme.Contains("themeb.theme"))
            {
                return SystemTheme.CapturedMotion;
            }

            if (currentTheme.Contains("themec.theme"))
            {
                return SystemTheme.Sunrise;
            }

            if (currentTheme.Contains("themed.theme"))
            {
                return SystemTheme.Flow;
            }
        }

        /*if (currentTheme.Contains("custom.theme"))
            return ; custom can be light or dark*/
        var rawAppsUseLightTheme = Registry.GetValue(
            "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
            "AppsUseLightTheme",
            1
        );

        if (rawAppsUseLightTheme is 0)
        {
            return SystemTheme.Dark;
        }
        else if (rawAppsUseLightTheme is 1)
        {
            return SystemTheme.Light;
        }

        var rawSystemUsesLightTheme =
            Registry.GetValue(
                "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                "SystemUsesLightTheme",
                1
            ) ?? 1;

        return rawSystemUsesLightTheme is 0 ? SystemTheme.Dark : SystemTheme.Light;
    }
    #endregion
}
