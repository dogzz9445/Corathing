// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Services;

/// <summary>
/// Windows 11 themes.
/// </summary>
public enum SystemTheme
{
    /// <summary>
    /// Unknown Windows theme.
    /// </summary>
    Unknown,

    /// <summary>
    /// Custom Windows theme.
    /// </summary>
    Custom,

    /// <summary>
    /// Default light theme.
    /// </summary>
    Light,

    /// <summary>
    /// Default dark theme.
    /// </summary>
    Dark,

    /// <summary>
    /// High-contrast theme: Desert
    /// </summary>
    HCWhite,

    /// <summary>
    /// High-contrast theme: Acquatic
    /// </summary>
    HCBlack,

    /// <summary>
    /// High-contrast theme: Dusk
    /// </summary>
    HC1,

    /// <summary>
    /// High-contrast theme: Nightsky
    /// </summary>
    HC2,

    /// <summary>
    /// Dark theme: Glow
    /// </summary>
    Glow,

    /// <summary>
    /// Dark theme: Captured Motion
    /// </summary>
    CapturedMotion,

    /// <summary>
    /// Light theme: Sunrise
    /// </summary>
    Sunrise,

    /// <summary>
    /// Light theme: Flow
    /// </summary>
    Flow
}

public enum WindowBackdropType
{
    /// <summary>
    /// No backdrop effect.
    /// </summary>
    None,

    /// <summary>
    /// Sets <c>DWMWA_SYSTEMBACKDROP_TYPE</c> to <see langword="0"></see>.
    /// </summary>
    Auto,

    /// <summary>
    /// Windows 11 Mica effect.
    /// </summary>
    Mica,

    /// <summary>
    /// Windows Acrylic effect.
    /// </summary>
    Acrylic,

    /// <summary>
    /// Windows 11 wallpaper blur effect.
    /// </summary>
    Tabbed
}

/// <summary>
/// Theme in which an application using WPF UI is displayed.
/// </summary>
public enum ApplicationTheme
{
    /// <summary>
    /// Unknown application theme.
    /// </summary>
    Unknown,

    /// <summary>
    /// Dark application theme.
    /// </summary>
    Dark,

    /// <summary>
    /// Light application theme.
    /// </summary>
    Light,

    /// <summary>
    /// High contract application theme.
    /// </summary>
    HighContrast
}


public interface IThemeService
{
    void Register(string stringNamespace, string lightUri, string darkUri);
    void Apply(ApplicationTheme theme,
        WindowBackdropType backgroundEffect = WindowBackdropType.Mica,
        bool updateAccent = true);
    void ApplySystemTheme();
    ApplicationTheme GetAppTheme();
    SystemTheme GetSystemTheme();

}
