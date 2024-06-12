using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows;

using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.Resources;
using Corathing.Organizer.Services;
using Corathing.Organizer.Utils;
using Corathing.Organizer.ViewModels;
using Corathing.Organizer.Views;
using Corathing.Widgets.Basics.Resources;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

using IThemeService = Corathing.Contracts.Services.IThemeService;
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

        //Application.Current.DispatcherUnhandledException += (sender, args) =>
        //{
        //    MessageBox.Show(args.Exception.Message, "Unhandled exception occured");
        //    //Logger.LogError(args.Exception, "Unhandled exception occured");
        //};

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
        appStateService.InitializeAsync().Wait();

        var appSettings = appStateService.GetAppSettings();
        var appPreferences = appStateService.GetAppPreferenceState();

        // Set the theme
        var theme = appStateService.GetAppPreferenceState().Theme ?? ApplicationTheme.Light;
        var themeService = Services.GetService<IThemeService>();
        if (themeService != null)
        {
            themeService.Register(
                "Corathing.Dashboards.WPF",
                "pack://application:,,,/Corathing.Dashboards.WPF;component/Themes/Light.xaml",
                "pack://application:,,,/Corathing.Dashboards.WPF;component/Themes/Dark.xaml"
                );
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

        LocalizationService.Instance.RegisterStringResourceManager("Corathing.Widgets.Basics",
            BasicWidgetStringResources.ResourceManager);

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
        new MainWindow().Show();
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

        // Build the configuration
        var configuration = BuildConfiguration(args);
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Wpf.Ui Service
        // TODO: This should be implmented with IDialogService (Corathing.Contracts.Services)
        // RoadMap 3, Content Presenter (Binding using Context)
        serviceCollection.AddSingleton<IContentDialogService, ContentDialogService>();

        // Register services
        serviceCollection.AddSingleton<IApplicationService, ApplicationService>();
        serviceCollection.AddSingleton<ISecretService, ModelVersionSecretService>();
        serviceCollection.AddSingleton<IAuthService, AuthService>();
        serviceCollection.AddSingleton<IAppStateService, AppStateService>();
        serviceCollection.AddSingleton<IPackageService, PackageService>();
        serviceCollection.AddSingleton<IThemeService, ThemeService>();
        serviceCollection.AddSingleton<ILocalizationService>(LocalizationService.Instance);

        // Register viewmodels
        serviceCollection.AddScoped<OrganizerSettingsViewModel>();
        serviceCollection.AddScoped<WidgetSettingsViewModel>();
        serviceCollection.AddScoped<ProjectSettingsViewModel>();

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
            }
        }

        return processExists;
    }
}
