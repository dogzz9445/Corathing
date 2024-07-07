using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;

using Microsoft.CodeAnalysis;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

[EntryCoraDataSource(
    dataSourceType: typeof(ExecutableAppDataSourceContext),
    optionType: typeof(ExecutableAppDataSourceOption),
    settingsContextType: typeof(ExecutableAppDataSourceOptionViewModel),
    name: "Executable App",
    description: "Execute an executable app with the selected files.",
    defaultTitle: "DefaultApp"
)]
public class ExecutableAppDataSourceContext : DataSourceContext, IDataSourceContext
{
    public string ExecutableAppPath { get; set; }
    public string CommandLineArguments { get; set; }

    public override void OnCreate(IServiceProvider services, DataSourceState state)
    {

    }

    public override void OnStateChanged(DataSourceState state)
    {
        if (state.CustomSettigns is not ExecutableAppDataSourceOption option)
        {
            return;
        }

        ExecutableAppPath = option.ExecutableAppPath;
        CommandLineArguments = option.CommandLineArguments;
    }

    public void Apply(DataSourceState state)
    {
    }

    public void OnDestroy(DataSourceState state)
    {
    }

    public void Execute(List<string> paths)
    {
        var arguments = paths.Select(path => $"{CommandLineArguments} {path}");

        foreach (var argument in arguments)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = ExecutableAppPath,
                Arguments = argument,
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                using (Process process = Process.Start(processStartInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
