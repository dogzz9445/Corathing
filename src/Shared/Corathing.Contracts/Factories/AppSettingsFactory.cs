﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Factories;

public static class AppSettingsFactory
{
    public static AppSettings Create()
        => new AppSettings()
        {
            UseGlobalConfiguration = false,
            UseAppPathConfiguration = true,
            CustomPath = "",
        };

    public static AppSettings Copy(this AppSettings appSettings)
        => new AppSettings()
        {
            UseGlobalConfiguration = appSettings.UseGlobalConfiguration,
            UseAppPathConfiguration = appSettings.UseAppPathConfiguration,
            CustomPath = appSettings.CustomPath,
        };

    public static AppSettings CopyWithUpdate(
        this AppSettings appSettings,
        bool? useGlobalConfiguration = null,
        bool? useAppPathConfiguration = null,
        string? customPath = null)
            => new AppSettings()
            {
                UseGlobalConfiguration = useGlobalConfiguration ?? appSettings.UseGlobalConfiguration,
                UseAppPathConfiguration = useAppPathConfiguration ?? appSettings.UseAppPathConfiguration,
                CustomPath = customPath ?? appSettings.CustomPath,
            };
}