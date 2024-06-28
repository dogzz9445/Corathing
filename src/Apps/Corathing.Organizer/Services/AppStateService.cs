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
using System.Windows.Shapes;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Configurations;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Factories;
using Corathing.Organizer.Models;

using Microsoft.Extensions.DependencyInjection;

using Smart.IO;

using Windows.ApplicationModel.Calls;

using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Corathing.Organizer.Services;

#pragma warning disable CS8601 // Possible null reference assignment.

public class AppStateService : IAppStateService
{
    #region Readonly Properties
    private readonly IServiceProvider _services;

    private const string AppSettingsFilename = "appsettings.json";
    private const string OrganizerSettingsFilename = "cora-organizer-settings.json";


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

    public AppStateService(IServiceProvider services)
    {
        _services = services;
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

    public List<ProjectState> GetProjects()
    {
        return GetAppDashboardState().Projects;
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

        return _cachedAppDashboardState.CashedProjects.TryGetValue(id, out project);
    }

    public bool TryGetWorkflow(Guid id, out WorkflowState workflow)
    {
        if (_cachedAppDashboardState == null)
            ReadOrCreateAppStateByAppSettings().Wait();

        return _cachedAppDashboardState.CashedWorkflows.TryGetValue(id, out workflow);
    }

    public bool TryGetWidget(Guid id, out WidgetState widget)
    {
        if (_cachedAppDashboardState == null)
            ReadOrCreateAppStateByAppSettings().Wait();

        return _cachedAppDashboardState.CashedWidgets.TryGetValue(id, out widget);
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

    public ProjectState CreateAddProject()
    {
        var project = ProjectState.Create();
        _cachedAppDashboardState.UpdateProject(project);
        return project;
    }

    public WorkflowState CreateAddWorkflow()
    {
        var workflow = WorkflowState.Create();
        _cachedAppDashboardState.UpdateWorkflow(workflow);
        return workflow;
    }

    public ProjectState CloneProject(Guid originalProjectId)
    {
        if (!TryGetProject(originalProjectId, out var originalProject))
        {
            // TODO:
            // Change Exception Type;
            throw new Exception();
        }
        return CloneProject(originalProject);
    }

    public ProjectState CloneProject(ProjectState originalProject)
    {
        var cloneProject = ProjectState.Create();
        foreach (var originalWorkflowId in originalProject.WorkflowIds)
        {
            var workflowState = CloneWorkflow(originalWorkflowId);
            cloneProject.WorkflowIds.Add(workflowState.Id);
        }
        return cloneProject;
    }

    public WorkflowState CloneWorkflow(Guid originalWorkflowId)
    {
        if (!TryGetWorkflow(originalWorkflowId, out var originalWorkflow))
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        return CloneWorkflow(originalWorkflow);
    }

    public WorkflowState CloneWorkflow(WorkflowState workflowState)
    {
        var cloneWorkflow = WorkflowState.Create();

        _cachedAppDashboardState.UpdateWorkflow(cloneWorkflow);
        return cloneWorkflow;
    }

    public void RemoveProject(Guid projectid)
    {
        if (!TryGetProject(projectid, out var project))
        {
            // TODO:
            // Change Exception Type;
            throw new Exception();
        }
        RemoveProject(project);
    }

    public async void RemoveProject(ProjectState project)
    {
        foreach (var workflowId in project.WorkflowIds)
        {
            RemoveWorkflow(workflowId);
        }
        _cachedAppDashboardState.RemoveProject(project);

        await PendingWriteAppState();
    }

    public void RemoveWorkflow(Guid workflowId)
    {
        if (!TryGetWorkflow(workflowId, out var workflow))
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        RemoveWorkflow(workflow);
    }

    public async void RemoveWorkflow(WorkflowState workflow)
    {
        foreach (var widgetId in workflow.WidgetIds)
        {
            RemoveWidget(widgetId);
        }
        _cachedAppDashboardState.RemoveWorkflow(workflow);

        await PendingWriteAppState();
    }

    public void RemoveWidget(Guid widgetId)
    {
        if (!TryGetWidget(widgetId, out var widget))
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        RemoveWidget(widget);
    }

    public async void RemoveWidget(WidgetState widget)
    {
        _cachedAppDashboardState.RemoveWidget(widget);

        await PendingWriteAppState();
    }

    #region Private Methods
    private string GetOrganizerSettingsFilename()
    {
        AppSettings appSettings = GetAppSettings();

        // Read or Create AppState Data
        if ((appSettings.UseCustomConfiguration ?? false) &&
            !string.IsNullOrEmpty(appSettings.CustomConfigurationFilename))
        {
            return appSettings.CustomConfigurationFilename;
        }
        else
        {
            var storageService = _services.GetService<IStorageService>();
            string appDataPath = storageService.GetAppDataPath();
            return Path.Combine(appDataPath, OrganizerSettingsFilename);
        }
    }


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
            using var fileStream = File.Open(AppSettingsFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            var json = await streamReader.ReadToEndAsync();
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

    private async Task ReadOrCreateAppStateByAppSettings()
    {
        await ReadOrCreateAppStateFromPath(GetOrganizerSettingsFilename());
    }

    private async Task ReadOrCreateAppStateFromPath(string path)
    {
        _isReading = true;
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
            using var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            var json = await streamReader.ReadToEndAsync();
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

            _cachedAppDashboardState.RefreshCache();
        }
        _isReading = false;
    }

    bool _isPending = false;
    bool _isReading = false;
    bool _isWriting = false;

    private async Task PendingWriteAppState()
    {
        if (_isPending)
            return;
        _isPending = true;

        while (_isWriting || _isReading)
        {
            await Task.Delay(1);
        }
        await WrtieAppState();

        _isPending = false;
    }

    public async void Flush()
    {
        await PendingWriteAppState();
    }

    public async void UpdateForce()
    {
        await WrtieAppState();
    }

    private async Task WrtieAppState()
    {
        _isWriting = true;
        using var fileStream = File.Open(GetOrganizerSettingsFilename(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        using var streamReader = new StreamReader(fileStream);
        var json = await streamReader.ReadToEndAsync();
        if (string.IsNullOrEmpty(json))
            json = AppStateJsonDomBase;

        var rootNode = JsonNode.Parse(json);

        rootNode["Preferences"].ReplaceWith(_cachedAppPreferenceState);
        rootNode["Packages"].ReplaceWith(_cachedAppPackageState);
        rootNode["Dashboards"].ReplaceWith(_cachedAppDashboardState);

        fileStream.SetLength(0);
        fileStream.Seek(0, SeekOrigin.Begin);
        await fileStream.WriteAsync(Encoding.UTF8.GetBytes(rootNode.ToJsonString(_serializerOptions)));
        _isWriting = false;
    }

    private async Task UpdateInternal(AppDashboardState appState)
    {

    }

    #endregion
}
#pragma warning restore CS8601 // Possible null reference assignment.
