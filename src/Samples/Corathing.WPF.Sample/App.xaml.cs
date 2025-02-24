using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

using Corathing.WPF.Sample.ProcessService;

namespace Corathing.WPF.Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public SecondInstanceService SecondInstanceService { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

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

        SecondInstanceService = new SecondInstanceService();
        SecondInstanceService.OnSecondInstanceRequest += (request) =>
        {
            switch (request)
            {
                case SecondInstanceService.SecondInstanceRequest.MaximizeWindow:
                    Current.Dispatcher.Invoke(() =>
                    {
                        if (Current.MainWindow.WindowState == WindowState.Minimized)
                        {
                            Current.MainWindow.WindowState = WindowState.Normal;
                        }
                        Current.MainWindow.Activate();
                    });
                    break;
            }
        };
        SecondInstanceService.StartAsync(new System.Threading.CancellationToken());
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        SecondInstanceService?.Dispose();
    }

    private static bool CheckIfProcessExists()
    {
        bool processExists = false;
        Process thisInstance = Process.GetCurrentProcess();
        if (Process.GetProcessesByName(thisInstance.ProcessName).Length > 1)
        {
            processExists = true;
            using (var clientPipe = NamedPipeServerStreamFactory.GetClientPipe())
            {
                clientPipe.Connect(1);
                clientPipe.Write(new byte[] { (byte)SecondInstanceService.SecondInstanceRequest.MaximizeWindow }, 0, 1);
            }
        }
        return processExists;
    }

}

