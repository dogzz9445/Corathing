using System.Runtime.InteropServices;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Services;

using Corathing.Widgets.Basics.Resources;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("0f7a4e2a-e050-4e43-881e-03a66ebded95")]

[assembly: AssemblyCoraPackageName(ApplicationLanguage.en_US, "Basic Widgets")]
[assembly: AssemblyCoraPackageName(ApplicationLanguage.ko_KR, "기본 위젯")]
[assembly: AssemblyCoraPackageDescription(ApplicationLanguage.en_US, "Provides basic widgets for Corathing.")]
[assembly: AssemblyCoraPackageDescription(ApplicationLanguage.ko_KR, "Corathing을 위한 기본 위젯을 제공합니다.")]

[assembly: AssemblyCoraPackageDataTemplate("DataTemplates.xaml")]
[assembly: AssemblyCoraPackageResourceManager(
    resourceType: typeof(BasicWidgetStringResources),
    nameofResourceManager: nameof(BasicWidgetStringResources.ResourceManager))]
