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
using System.Windows;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Factories;
using Corathing.Contracts.Services;
using Corathing.Organizer.Models;

namespace Corathing.Organizer.Services;

#pragma warning disable CS8601 // Possible null reference assignment.

public class AppStateService : IAppStateService
{
    #region Readonly Properties
    private const string AppSettingsFilename = "appsettings.json";
    private const string CorathingSettingsFilename = "corathing-settings.json";

    private const string AppStateJsonDomBase = """
        {
            "Preferences": {},
            "Packages": {},
            "Dashboards": {}
        }
        """;

    private readonly JsonSerializerOptions _serializerOptions =
        new JsonSerializerOptions()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            WriteIndented = true,
        };

    private readonly JsonDocumentOptions _documentOptions =
        new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip
        };
    #endregion

    #region Cached Properties
    private AppSettings? _cachedAppSettings;
    private AppPreferenceState? _cachedAppPreferenceState;
    private AppPackageState? _cachedAppPackageState;
    private AppDashboardState? _cachedAppDashboardState;
    private JsonNode? _cachedAppStateDom;
    private object _lockAppState;
    #endregion

    private void SaveAppSate()
    {
        lock (_lockAppState)
        {

        }
    }

    private void ReadAppSate()
    {

    }

    public async Task InitializeAsync()
    {
        await ReadOrCreateAppSettingsFromAppPath();
        await ReadOrCreateAppStateByAppSettings();
    }

    public AppSettings GetAppSettings()
    {
        if (_cachedAppSettings != null)
            return _cachedAppSettings;

        ReadOrCreateAppSettingsFromAppPath().Wait();
        return _cachedAppSettings;
    }

    public async void UpdateAppSettings(AppSettings appSettings)
    {
        _cachedAppSettings = appSettings;
        await WriteAppSettingsToAppPath(appSettings);

        await ReadOrCreateAppStateByAppSettings();
    }

    public AppPreferenceState GetAppPreferenceState()
    {
        if (_cachedAppPreferenceState != null)
            return _cachedAppPreferenceState;

        // FIXME:
        // 없을 경우 Read 하도록
        return _cachedAppPreferenceState;
    }

    public AppPackageState GetAppPackageState()
    {
        if (_cachedAppPackageState != null)
            return _cachedAppPackageState;

        // FIXME:
        // 없을 경우 Read 하도록
        return _cachedAppPackageState;
    }

    public AppDashboardState GetAppDashboardState()
    {
        if (_cachedAppDashboardState != null)
            return _cachedAppDashboardState;

        // FIXME:
        // 없을 경우 Read 하도록
        return _cachedAppDashboardState;
    }

    public bool TryGetProject(Guid id, out ProjectState project)
    {
        if (_cachedAppDashboardState == null)
            ReadOrCreateAppStateByAppSettings().Wait();

        project = new ProjectState();
        return true;
    }

    public bool TryGetWorkflow(Guid id, out WorkflowState workflow)
    {
        if (_cachedAppDashboardState == null)
            ReadOrCreateAppStateByAppSettings().Wait();

        workflow = new WorkflowState();
        return true;
    }

    public bool TryGetWidget(Guid id, out WidgetState option)
    {
        if (_cachedAppDashboardState == null)
            ReadOrCreateAppStateByAppSettings().Wait();

        option = default;
        return true;
    }

    public ProjectState GetOrAddProject(Guid? id = null)
    {
        return default;
    }


    public async void UpdateProject(ProjectState project)
    {
        if (_cachedAppDashboardState == null)
            await ReadOrCreateAppStateByAppSettings();

        if (project == null)
        {
            MessageBox.Show("Project is null");
            return;
        }

        _cachedAppDashboardState.UpdateProject(project);

        await PendingWriteAppState();
    }

    public async void UpdateWorkflow(WorkflowState workflow)
    {
        if (_cachedAppDashboardState == null)
            await ReadOrCreateAppStateByAppSettings();

        if (workflow == null)
        {
            MessageBox.Show("Workflow is null");
            return;
        }

        _cachedAppDashboardState.UpdateWorkflow(workflow);

        await PendingWriteAppState();
    }

    public async void UpdateWidget(WidgetState widget)
    {
        if (_cachedAppDashboardState == null)
            await ReadOrCreateAppStateByAppSettings();

        if (widget == null)
        {
            MessageBox.Show("Widget is null");
            return;
        }

        _cachedAppDashboardState.UpdateWidget(widget);

        await PendingWriteAppState();
    }

    public ProjectState AddProject()
    {

        return 
    }

    public WorkflowState AddWorkflow()
    {

    }

    public WidgetState AddWidgetState()
    {

    }

    #region Private Methods

    private async Task<AppSettings> ReadOrCreateAppSettingsFromAppPath()
    {
        if (!File.Exists(AppSettingsFilename))
        {
            var appSettings = AppSettingsFactory.Create();
            var json = JsonSerializer.Serialize(appSettings);
            await File.WriteAllTextAsync(AppSettingsFilename, json);
            _cachedAppSettings = appSettings;
            return appSettings;
        }
        else
        {
            var json = File.ReadAllText(AppSettingsFilename);
            using var document = JsonDocument.Parse(json, _documentOptions);
            var appSettings = document.RootElement
                .GetProperty("Corathing")
                .GetProperty("Organizer")
                .Deserialize<AppSettings>();
            _cachedAppSettings = appSettings;
            return appSettings;
        }
    }

    private async Task WriteAppSettingsToAppPath(AppSettings appSettings)
    {
        if (appSettings == null)
            return;

        string jsonString = await File.ReadAllTextAsync(AppSettingsFilename);

        var rootNode = JsonNode.Parse(jsonString);

        rootNode["Corathing"]["Organizer"].ReplaceWith(appSettings);
        var json = rootNode.ToJsonString(_serializerOptions);

        await File.WriteAllTextAsync(AppSettingsFilename, json);
    }

    /// <summary>
    /// AppState 파일 이름을 가져옴
    /// </summary>
    /// <returns></returns>
    private string GetAppStatePathByAppSettings()
    {
        // Read or Create AppState Data
        if (_cachedAppSettings.UseGlobalConfiguration ?? false)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                CorathingSettingsFilename
                );
        }
        else if (_cachedAppSettings.UseAppPathConfiguration ?? false)
        {
            return CorathingSettingsFilename;
        }
        else if (!string.IsNullOrEmpty(_cachedAppSettings.CustomConfigurationFilename))
        {
            return _cachedAppSettings.CustomConfigurationFilename;
        }
        else
        {
            return CorathingSettingsFilename;
        }
    }

    private async Task ReadOrCreateAppStateByAppSettings()
    {
        var path = GetAppStatePathByAppSettings();
        await ReadOrCreateAppStateFromPath(path);
    }

    private async Task ReadOrCreateAppStateFromPath(string path)
    {
        if (!File.Exists(path))
        {
            if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            }
            _cachedAppPreferenceState = AppPreferenceStateFactory.Create();
            _cachedAppPackageState = AppPackageStateFactory.Create();
            _cachedAppDashboardState = AppDashboardStateFactory.Create();

            JsonNode rootNode = JsonNode.Parse(AppStateJsonDomBase);
            rootNode["Preferences"].ReplaceWith(_cachedAppPreferenceState);
            rootNode["Packages"].ReplaceWith(_cachedAppPackageState);
            rootNode["Dashboards"].ReplaceWith(_cachedAppDashboardState);

            await File.WriteAllTextAsync(path, rootNode.ToJsonString(_serializerOptions));
        }
        else
        {
            var json = File.ReadAllText(path);
            using var document = JsonDocument.Parse(json, _documentOptions);

            // TODO:
            // Document 에 내용이 없으면 리턴 가능하게
            if (document == null)
                return;

            _cachedAppPreferenceState = document.RootElement
                .GetProperty("Preferences")
                .Deserialize<AppPreferenceState>();
            _cachedAppPackageState = document.RootElement
                .GetProperty("Packages")
                .Deserialize<AppPackageState>();
            _cachedAppDashboardState = document.RootElement
                .GetProperty("Dashboards")
                .Deserialize<AppDashboardState>();
        }
    }

    private async Task PendingWriteAppState()
    {
        // lock or Write
        // await WrtieAppState();
    }

    public async void UpdateForce()
    {
        await WrtieAppState();
    }

    private async Task WrtieAppState()
    {
        string jsonString = await File.ReadAllTextAsync(GetAppStatePathByAppSettings());
        if (string.IsNullOrEmpty(jsonString))
            jsonString = AppStateJsonDomBase;

        var rootNode = JsonNode.Parse(jsonString);

        rootNode["Preferences"].ReplaceWith(_cachedAppPreferenceState);
        rootNode["Packages"].ReplaceWith(_cachedAppPackageState);
        rootNode["Dashboards"].ReplaceWith(_cachedAppDashboardState);

        await File.WriteAllTextAsync(GetAppStatePathByAppSettings(),
            rootNode.ToJsonString(_serializerOptions));
    }



    private async Task UpdateInternal(AppDashboardState appState)
    {

    }

    #endregion
}
#pragma warning restore CS8601 // Possible null reference assignment.
