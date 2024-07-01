using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.Resources;

[assembly: AssemblyCoraPackageName(ApplicationLanguage.en_US, "Basic Widgets")]
[assembly: AssemblyCoraPackageName(ApplicationLanguage.ko_KR, "기본 위젯")]
[assembly: AssemblyCoraPackageDescription(ApplicationLanguage.en_US, "Provides basic widgets for Corathing.")]
[assembly: AssemblyCoraPackageDescription(ApplicationLanguage.ko_KR, "Corathing을 위한 기본 위젯을 제공합니다.")]

[assembly: AssemblyCoraPackageDataTemplate("DataTemplates.xaml")]
[assembly: AssemblyCoraPackageResourceManager(
    resourceType: typeof(BasicWidgetStringResources),
    nameofResourceManager: nameof(BasicWidgetStringResources.ResourceManager))]
