using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

namespace Corathing.Widgets.Basics.DataSources.FileStorages;

[EntryCoraDataSource(
    dataSourceType: typeof(FileStorageDataSourceContext),
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
public class FileStorageDataSourceContext : DataSourceContext
{
}
