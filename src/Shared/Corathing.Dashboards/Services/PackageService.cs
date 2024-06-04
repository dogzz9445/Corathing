using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

namespace Corathing.Dashboards.Services;

public class ProxyDomain : MarshalByRefObject
{
    public Assembly? GetAssembly(string assemblyPath)
    {
        try
        {
            return Assembly.Load(assemblyPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
public class PackageService : IPackageService
{
    /// <summary>
    /// Gets or sets the available widgets.
    /// </summary>
    /// <value>The available widgets.</value>
    private readonly List<WidgetGenerator> _availableWidgets = new List<WidgetGenerator>();
    private readonly IServiceProvider _services;

    public PackageService(IServiceProvider services)
    {
        _services = services;
    }


    public void RegisterWidgets(List<WidgetGenerator> widgets)
    {
        _availableWidgets.AddRange(widgets);
    }
    public List<WidgetGenerator> GetAvailableWidgets()
    {
        return _availableWidgets;
    }

    public void LoadWidgetsFromDLL(string pathDLL)
    {
        Assembly a = Assembly.LoadFrom(pathDLL);
        var types = a.GetTypes().Where(t => typeof(WidgetContext).IsAssignableFrom(t));
        List<WidgetGenerator> widgets = new List<WidgetGenerator>();
        // TODO:
        // 도메인 프록시 문제 해결 필요
        //var setup = new AppDomainSetup
        //{
        //    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
        //    PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        //};
        foreach (var type in types)
        {
            System.Reflection.MemberInfo info = type;
            var attributes = info.GetCustomAttributes(true);

            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is WidgetAttribute)
                {
                    var attribute = ((WidgetAttribute)attributes[i]);
                    attribute.RegisterServices(_services);
                    widgets.Add(attribute.WidgetGenerator);
                }
            }
        }

        RegisterWidgets(widgets);
    }
}
