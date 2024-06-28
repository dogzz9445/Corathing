using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows;

using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Models;
using Corathing.Organizer.Resources;
using Corathing.Organizer.Services;
using Corathing.Organizer.Utils;
using Corathing.Organizer.ViewModels;
using Corathing.Organizer.Views;
using Corathing.Widgets.Basics.Resources;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

using Application = System.Windows.Application;
using INavigationDialogService = Corathing.Contracts.Services.INavigationDialogService;
using IThemeService = Corathing.Contracts.Services.IThemeService;
using MessageBox = System.Windows.MessageBox;
using NavigationDialogService = Corathing.Organizer.Services.NavigationDialogService;
using ThemeService = Corathing.Organizer.Services.ThemeService;

namespace Corathing.Organizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    /// <summary>
    /// Gets the current <see cref="App"/>  instance of the application
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance of the application
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 오류 발생 시 처리
        Application.Current.DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(args.Exception.Message, "Unhandled exception occured");
            // FIXME:
            // Logger 사용
            //Logger.LogError(args.Exception, "Unhandled exception occured");
        };

        // 같은 이름의 다른 프로세스가 실행중인지 확인하고, 실행중이면 종료
        if (CheckIfProcessExists())
        {
            MessageBox.Show(
                "Another instance of the application is already running.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }

        Services = ConfigureServices(e.Args);

        var appStateService = Services.GetService<IAppStateService>();
        await appStateService.InitializeAsync();

        var appSettings = appStateService.GetAppSettings();
        var appPreferences = appStateService.GetAppPreferenceState();
        var appPackages = appStateService.GetAppPackageState();
        var appDashboards = appStateService.GetAppDashboardState();

        // Set the theme
        var theme = appStateService.GetAppPreferenceState().Theme ?? ApplicationTheme.Light;
        var themeService = Services.GetService<IThemeService>();
        if (themeService != null)
        {
            themeService.Register(
                "MaterialDesignThemes.Wpf",
                "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml",
                "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml"
                );
            themeService.Register(
                "MahApps.Metro",
                "pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml",
                "pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Cyan.xaml"
                );
            themeService.Register(
                "Corathing.UI.WPF",
                "pack://application:,,,/Corathing.UI.WPF;component/Themes/Light.xaml",
                "pack://application:,,,/Corathing.UI.WPF;component/Themes/Dark.xaml"
                );

            if (appPreferences.UseSystemTheme)
                themeService.ApplySystemTheme();
            else
                themeService.Apply(theme);
        }


        // --------------------------------------------------------------------------
        // Available Widgets
        // --------------------------------------------------------------------------
        IPackageService packageService = Services.GetService<IPackageService>();
        packageService.LoadWidgetsFromDLL("Corathing.Widgets.Basics.dll");
        //widgetService.LoadWidgetsFromDLL("DDT.Core.WidgetSystems.DefaultWidgets.dll");
        //widgetService.RegisterWidgets(new List<WidgetGenerator> { new WidgetGenerator() });

        LocalizationService.Instance.RegisterStringResourceManager("Corathing.Organizer",
            CorathingOrganizerLocalizationStringResources.ResourceManager);

        // --------------------------------------------------------------------------
        // Configure authentication
        // --------------------------------------------------------------------------
        IAuthService authService = App.Current.Services.GetService<IAuthService>();
        if (authService != null && authService.UseAuthService)
        {
            //var loginWindow = new BaseWindow();
            //loginWindow.Content = new LoginView();
            //loginWindow.Owner = Window.GetWindow(this);
            //loginWindow.ShowDialog();
            //if (loginWindow.DialogResult == false)
            //{
            //    // System.Shutdown
            //}
        }

        // Create a new MainWindow and set its DataContext to a new MainWindowViewModel which binds the view to the viewmodel
        var window = Services.GetService<MainWindow>();
        window.Show();
    }

    private static IConfigurationRoot BuildConfiguration(string[] args)
    {
        // Create and build a configuration builder
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());
            //.AddAppSettingsJsonFileByEnvironmentVariables()
            //.AddEnvironmentVariables()
            //.AddEntityConfiguration()
            //.AddCommandLine(args);

        return builder.Build();
    }

    private static IServiceProvider ConfigureServices(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        // --------------------------------------------------------------------------
        // Build the configuration
        // --------------------------------------------------------------------------
        var configuration = BuildConfiguration(args);
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // TODO:
        // Currently, use Wpf.Ui service,
        // This should be implmented with IDialogService (Corathing.Contracts.Services)
        // RoadMap 3, Content Presenter (Binding using Context)
        //serviceCollection.AddSingleton<IContentDialogService, ContentDialogService>();

        serviceCollection.AddSingleton<MainWindow>();
        serviceCollection.AddSingleton<MainViewModel>();

        // --------------------------------------------------------------------------
        // Register services
        // --------------------------------------------------------------------------
        serviceCollection.AddSingleton<IApplicationService, ApplicationService>();
        serviceCollection.AddSingleton<IAppStateService, AppStateService>();
        serviceCollection.AddSingleton<IAuthService, AuthService>();
        serviceCollection.AddSingleton<IDialogService, DialogService>();
        serviceCollection.AddSingleton<ILocalizationService>(LocalizationService.Instance);
        serviceCollection.AddSingleton<INavigationService, NavigationService>();
        serviceCollection.AddSingleton<IPageService, PageService>();
        serviceCollection.AddSingleton<IContentDialogService, ContentDialogService>();
        serviceCollection.AddSingleton<INavigationDialogService, NavigationDialogService>();
        serviceCollection.AddSingleton<IPackageService, PackageService>();
        serviceCollection.AddSingleton<IResourceDictionaryService, ResourceDictionaryService>();
        serviceCollection.AddSingleton<ISecretService, ModelVersionSecretService>();
        serviceCollection.AddSingleton<IStorageService, StorageService>();
        serviceCollection.AddSingleton<IThemeService, ThemeService>();

        // --------------------------------------------------------------------------
        // Navigation Service
        // --------------------------------------------------------------------------
        serviceCollection.AddSingleton<NavigationDialogView>();
        serviceCollection.AddSingleton<NavigationDialogViewModel>();
        serviceCollection.AddTransient<MultiLevelNavigationPage>();
        serviceCollection.AddTransient<MultiLevelNavigationViewModel>();

        // --------------------------------------------------------------------------
        // Register views and viewmodels
        // --------------------------------------------------------------------------
        serviceCollection.AddScoped<OrganizerSettingsView>();
        serviceCollection.AddScoped<OrganizerSettingsViewModel>();
        serviceCollection.AddScoped<WidgetSettingsViewModel>();
        serviceCollection.AddScoped<ProjectSettingsViewModel>();
        serviceCollection.AddScoped<WorkflowSettingsViewModel>();
        serviceCollection.AddTransient<ProjectContext>();
        serviceCollection.AddTransient<ProjectSettingsContext>();
        serviceCollection.AddTransient<WorkflowContext>();


        // TODO:
        // Logger 및 Localizer 설정
        //Logger.Configure(configuration);
        //Localizer.Configure(configuration);

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Checks if there is already an instance of Openhardwaremonitor running and brings up its window
    /// in case its minimized or as icon in taskbar
    /// </summary>
    private static bool CheckIfProcessExists()
    {
        bool processExists = false;
        Process thisInstance = Process.GetCurrentProcess();
        if (Process.GetProcessesByName(thisInstance.ProcessName).Length > 1)
        {
            processExists = true;
            using (var clientPipe = InterprocessCommunicationFactory.GetClientPipe())
            {
                clientPipe.Connect();
                clientPipe.Write(new byte[] { (byte)SecondInstanceService.SecondInstanceRequest.MaximizeWindow }, 0, 1);
                clientPipe.Flush();
            }
        }

        return processExists;
    }
}
