using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.Services;

public class StorageService : IStorageService
{
    private readonly IServiceProvider _services;

    // 1. Global Settings
    // Roaming
    // - Corathing
    //   - AppData
    //     - Current
    //       - cora-organizer-settings.json
    //       - cora-organizer-db.db
    //       - Packages
    //         - CachedNugets
    //           - Cora-Widgets-Basics.nuget
    //             - cora-module-entry.json
    //       - Entities
    //         - 81u68ug6txxwpbqz4znzgq3hfa
    //         - sseik38xykrr5n99zedj96nsoy
    //     - Packages
    //       - Caches
    //         - Cora-Widgets-Basics.nuget
    //           - cora-module-entry.json

    // 2. Local Settings
    // CorathingOrganizer
    // - AppData
    //   - Current
    //     - cora-organizer-settings.json
    //     - cora-organizer-db.db
    //     - Packages
    //       - CachedNugets
    //         - Cora-Widgets-Basics.nuget
    //           - cora-module-entry.json
    //     - Entities
    //       - 81u68ug6txxwpbqz4znzgq3hfa
    //       - sseik38xykrr5n99zedj96nsoy
    //   - Packages
    //     - Caches
    //       - Cora-Widgets-Basics.nuget
    //         - cora-module-entry.json

    // 3. Custom Path Settings
    // CorathingOrganizer
    // - AppData
    //   - Packages
    //     - Caches
    //       - Cora-Widgets-Basics.nuget
    //         - cora-module-entry.json
    // Custom File Path
    // - cora-organizer-settings.json
    // - cora-organizer-db.db
    // - Packages
    //   - CachedNugets
    //     - Cora-Widgets-Basics.nuget
    //       - cora-module-entry.json
    // - Entities
    //   - 81u68ug6txxwpbqz4znzgq3hfa
    //   - sseik38xykrr5n99zedj96nsoy

    private const string RoamingAppDataFolderName = "Corathing";
    private const string AppDataFolderName = "AppData";
    private const string PackageFolderName = "Packages";
    private const string OrganizerSettingsFolderName = "Current";
    private const string EntitiesFolderName = "Entities";

    public StorageService(IServiceProvider services)
    {
        _services = services;
    }

    public string GetAppDataPath()
    {
        var appStateService = _services.GetService<IAppStateService>();
        var appSettings = appStateService.GetAppSettings();

        // Read or Create AppState Data
        if (appSettings.UseGlobalConfiguration ?? false)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                RoamingAppDataFolderName,
                AppDataFolderName,
                OrganizerSettingsFolderName
                );
        }
        else if (appSettings.UseAppPathConfiguration ?? false)
        {
            return Path.Combine(
                AppDataFolderName,
                OrganizerSettingsFolderName
                );
        }
        else if (!string.IsNullOrEmpty(appSettings.CustomConfigurationFilename))
        {
            return Path.GetDirectoryName(appSettings.CustomConfigurationFilename);
        }
        else
        {
            return Path.Combine(
                AppDataFolderName,
                OrganizerSettingsFolderName
                );
        }
    }

    public string GetAppPackagePath()
    {
        var appStateService = _services.GetService<IAppStateService>();
        var appSettings = appStateService.GetAppSettings();

        // Read or Create AppState Data
        if (appSettings.UseGlobalConfiguration ?? false)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                RoamingAppDataFolderName,
                AppDataFolderName,
                PackageFolderName
                );
        }
        else if ((appSettings.UseCustomConfiguration ?? false) &&
            !string.IsNullOrEmpty(appSettings.CustomConfigurationFilename))
        {
            return Path.GetDirectoryName(appSettings.CustomConfigurationFilename);
        }
        else // Whene use local path
        {
            return Path.Combine(
                AppDataFolderName,
                PackageFolderName
                );
        }
    }

    public string GetEntityFolder(IEntity entity)
    {
        return Path.Combine(
            GetAppDataPath(),
            EntitiesFolderName,
            entity.Id.ToString().Replace("-", "")
            );
    }

    public FileStream OpenFile(IEntity entity, string path, FileMode mode)
    {
        return File.Open(
            Path.Combine(GetEntityFolder(entity), path), mode);
    }

    public void Delete(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<StorageHandleArgs<T>> ReadAsync<T>(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<StorageHandleArgs<T>> SaveAsync<T>(string filename, T content)
    {
        throw new NotImplementedException();
    }
}
