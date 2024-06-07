using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Factories;
using Corathing.Contracts.Services;

namespace Corathing.Organizer.Services;

public class AppStateService : IAppStateService
{
    private AppSettings _cachedAppSettings;
    private AppState _cachedAppState;

    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        WriteIndented = true,
    };

    private JsonWriterOptions _writerOptions = new JsonWriterOptions
    {
        Indented = true
    };

    public bool TryGetProject(Guid id, out ProjectState project)
    {
        project = new ProjectState();
        return true;
    }

    public bool TryGetWidgetOption<T>(Guid id, out T option)
    {
        option = default;
        return true;
    }

    public bool TryGetWorkflow(Guid id, out WorkflowState workflow)
    {
        workflow = new WorkflowState();
        return true;
    }

    public AppSettings GetAppSettings()
    {
        ReadAppSettingsFromAppPath();
        return _cachedAppSettings.Copy();
    }


    /// <summary>
    /// Update Key and Value AppSettings
    /// <see cref="AppSettings"/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void UpdateAppSettings(AppSettings appSettings)
    {
        _cachedAppSettings = appSettings;
        WriteAppSettingsToAppPath(appSettings);

        // Read or Create AppState Data
        if (_cachedAppSettings.UseGlobalConfiguration ?? false)
        {
            ReadOrCreateAppState(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "corathing-widgetsettings.json"
                ));
        }
        else if (_cachedAppSettings.UseAppPathConfiguration ?? false)
        {
            ReadOrCreateAppState("corathing-widgetsettings.json");
        }
        else if (!string.IsNullOrEmpty(_cachedAppSettings.CustomPath))
        {
            ReadOrCreateAppState(_cachedAppSettings.CustomPath);
        }
        else
        {
            ReadOrCreateAppState("corathing-widgetsettings.json");
        }
    }

    private void ReadOrCreateAppState(string path)
    {
        if (!File.Exists(path))
        {
            if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            }
            using var streamWriter = File.CreateText(path);
            var appState = AppStateFactory.Create();
            var json = JsonSerializer.Serialize(appState);
            streamWriter.Write(json);
        }
        else
        {
            var json = File.ReadAllText(path);
            using var document = JsonDocument.Parse(json,
                new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
            _cachedAppState = document.RootElement.Deserialize<AppState>();
        }
    }

    public void UpdateOrAdd(Guid id, object value)
    {
    }

    public void UpdateOverwrite(Guid id, object value)
    {
    }

    #region Private Methods
    private AppSettings ReadAppSettingsFromAppPath()
    {
        if (_cachedAppSettings != null)
            return _cachedAppSettings;

        var filePath = "appsettings.json";
        var json = File.ReadAllText(filePath);
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        var appSettings = document.RootElement
            .GetProperty("Corathing")
            .GetProperty("Organizer")
            .Deserialize<AppSettings>();
        _cachedAppSettings = appSettings;
        return appSettings;
    }

    private async void WriteAppSettingsToAppPath(AppSettings appSettings)
    {
        if (appSettings == null)
            return;

        var filePath = "appsettings.json";

        string jsonString = await File.ReadAllTextAsync(filePath);

        var rootNode = JsonNode.Parse(jsonString);

        rootNode["Corathing"]["Organizer"].ReplaceWith(appSettings);
        var json = rootNode.ToJsonString(_serializerOptions);

        await File.WriteAllTextAsync(filePath, json);
    }

    #endregion
}
