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
using Corathing.Contracts.Services;

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
[EntryCoraDataSourceDefaultTitle(ApplicationLanguage.en_US, "Default App")]
[EntryCoraDataSourceDefaultTitle(ApplicationLanguage.ko_KR, "기본 앱")]
[EntryCoraDataSourceName(ApplicationLanguage.en_US, "Executable App")]
[EntryCoraDataSourceName(ApplicationLanguage.ko_KR, "실행 앱")]
[EntryCoraDataSourceDescription(ApplicationLanguage.en_US, "Execute an executable app with the selected files")]
[EntryCoraDataSourceDescription(ApplicationLanguage.ko_KR, "실행 앱을 통해 선택된 파일들을 실행")]
public class ExecutableAppDataSourceContext : DataSourceContext
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

    public void Execute(List<string> paths)
    {
        var arguments = paths.Select(path => $"{CommandLineArguments} {path}");

        foreach (var argument in arguments)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = ExecutableAppPath,
                Arguments = argument,
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                using (var process = Process.Start(processStartInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
