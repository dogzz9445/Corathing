# Contributing

## 요구사항
- .NET 10 SDK
- Windows 10 (19041) 이상
- Visual Studio 2022 17.8+ 또는 Rider (선택)

## 리소스 파일(.resx) 수정 시
- `.resx` 에 키를 추가/수정하면 됩니다. `.Designer.cs` 를 직접 수정할 필요가 없습니다.
- Localization 클래스(`CorathingOrganizerLocalizationStringResources`, `BasicWidgetStringResources`)는 `ResourceManager` 래퍼만 제공하는 수동 작성 파일이므로 `.resx` 변경과 무관하게 그대로 둡니다.

## 커밋
- Nerdbank.GitVersioning 을 사용하므로 fetch-depth 가 얕으면 로컬 버전 계산이 틀어질 수 있습니다. 깊은 clone 을 권장합니다.
- 빌드/CI 는 `dotnet restore`, `dotnet build Corathing.sln`, `dotnet publish src/Apps/Corathing.Organizer.WPF` 경로로 동작합니다.
