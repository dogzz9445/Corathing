using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Application = System.Windows.Application;

namespace Corathing.Organizer.UnitTests;


[TestClass]
public class AppUnitTest
{
    private static Thread RunApplicationAction(Action<Application> action)
    {
        Thread thread = new Thread(() =>
        {
            var application = new App();
            Application.ResourceAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            application.InitializeComponent();
            application.Dispatcher.InvokeAsync(() =>
            {
                _internalRunApplicationAction(action, application);
                action(application);
            }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            application.Run();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return thread;
    }

    private static async void _internalRunApplicationAction(Action<Application> action, Application application)
    {
        Window window = application.MainWindow;
        await Task.Delay(TimeSpan.FromSeconds(5));

        action(application);

        await Task.Delay(TimeSpan.FromSeconds(5));
        window.Close();
    }

    [TestMethod]
    public void App_ConfigureServices_Test()
    {
        //var thread = RunApplicationAction(application =>
        //{

        //});
        //thread.Join();
    }
}
