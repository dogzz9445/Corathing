# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Corathing은 Windows용 위젯 대시보드 데스크탑 애플리케이션입니다. Freeter(UI/UX)와 Grafana(대시보드 모니터링)에서 영감을 받아, 사용자가 위젯을 배치/리사이즈/설정해서 자신만의 작업공간을 구성할 수 있게 합니다. WPF가 메인 UI 프레임워크이고, 플러그인 형태로 위젯/데이터소스를 DLL 또는 NuGet 패키지에서 로드합니다.

- 메인 실행 프로젝트: [src/Apps/Corathing.Organizer.WPF](src/Apps/Corathing.Organizer.WPF/) — `WinExe`, 진입점은 [App.xaml.cs](src/Apps/Corathing.Organizer.WPF/App.xaml.cs)
- 솔루션 파일: [Corathing.sln](Corathing.sln)
- 모든 프로젝트는 `.NET 10` 타깃. 로컬 빌드에 .NET 10 SDK 필요.

## 빌드 / 실행 명령

```bash
# 복원 + 빌드 (솔루션 전체)
dotnet restore Corathing.sln
dotnet build Corathing.sln -c Debug

# 메인 앱 실행
dotnet run --project src/Apps/Corathing.Organizer.WPF/Corathing.Organizer.WPF.csproj

# 특정 위젯 프로젝트만 빌드 (Organizer.WPF의 pre-build target이 자동으로 하긴 하지만 개별 확인용)
dotnet build src/Widgets/Corathing.Widgets.Basics/Corathing.Widgets.Basics.csproj

# 샘플 실행 (위젯 호스트 단독 테스트)
dotnet run --project src/Samples/Corathing.Dashboards.WPF.Sample/Corathing.Dashboards.WPF.Sample.csproj
```

테스트 프로젝트는 **현재 존재하지 않습니다**. CI 워크플로우([.github/workflows/ci.yml](.github/workflows/ci.yml)) 는 dotnet restore + build + publish 만 수행하며, 단위 테스트 프로젝트가 추가되면 `dotnet test` 단계를 넣도록 TODO 표기가 되어 있습니다.

## 위젯 DLL 참조 패턴 (주의)

[Corathing.Organizer.WPF.csproj](src/Apps/Corathing.Organizer.WPF/Corathing.Organizer.WPF.csproj) 는 `Corathing.Widgets.Basics` 를 **ProjectReference 가 아닌 DLL `<Reference>`** 로 참조합니다. 의도된 구조입니다 — 위젯은 플러그인이라 `PackageService.LoadDLL("Corathing.Widgets.Basics.dll")` 로 런타임에 리플렉션 로드되고, 컴파일 타임 종속을 만들고 싶지 않기 때문. 빌드 순서는 `BuildDependentProject` 타깃이 Widgets.Basics 를 먼저 빌드해 `bin/$(Configuration)/net10.0-windows10.0.19041.0/` 에 DLL 을 만들고, HintPath 는 `$(Configuration)` MSBuild 변수로 해당 경로를 가리킵니다. 이 관계를 깨지 않도록 주의하세요.

## 상위 아키텍처

플러그인 기반 위젯 시스템입니다. 레이어 구조는 다음과 같습니다:

```
Widgets (플러그인: Corathing.Widgets.Basics …)   ← 속성 기반 리플렉션 등록
        ▲
        │ [EntryCoraWidget] / [EntryCoraDataSource]
        │
Contracts + Contracts.Utils  ← 추상화, 상태 DTO, 제너레이터/팩토리
        ▲
        │
Dashboards (UI 중립) → Dashboards.WPF (호스트 컨트롤)
Organizer (Package/DataSource 서비스) → Organizer.WPF (DI, 뷰, 뷰모델)
```

### 핵심 흐름 — 앱 부팅 시

[App.xaml.cs](src/Apps/Corathing.Organizer.WPF/App.xaml.cs) 의 `ConfigureServices` 가 DI 컨테이너를 구성하고, 다음 순서로 초기화됩니다:

1. `IAppStateService.InitializeAsync()` — `%AppData%\Corathing` (또는 사용자 지정 경로) 하위의 `AppData/Current/cora-organizer-settings.json` 을 읽어 **Project → Workflow → Widget / DataSource** 트리를 메모리에 로드.
2. `IThemeService` — Light/Dark + Windows 11 backdrop (Mica/Acrylic) 적용.
3. `IPackageService.LoadDLL("Corathing.Widgets.Basics.dll")` — 리플렉션으로 `[EntryCoraWidget]`, `[EntryCoraDataSource]`, 어셈블리 레벨 `[AssemblyCoraPackage*]` 속성을 읽어 **CoraWidgetInfo / CoraDataSourceInfo / CoraPackageInfo** 사전에 등록. NuGet 동적 로드도 같은 서비스(`LoadNugetFromFile`, `LoadNugetFromWebAsync`)에서 처리.
4. `ILocalizationService.RegisterStringResourceManager(...)` — 각 어셈블리의 .resx 리소스를 등록. `LocalizationService` 는 싱글턴(`LocalizationService.Instance`)이며 [src/Shared/Corathing.Dashboards.WPF/Services/LocalizationService.cs](src/Shared/Corathing.Dashboards.WPF/Services/LocalizationService.cs) 에 위치.
5. `MainWindow` 표시.

### 위젯 / 데이터소스 수명주기 (중요)

모든 위젯은 **`WidgetContext` 추상클래스**([src/Shared/Corathing.Contracts/Bases/WidgetContext.cs](src/Shared/Corathing.Contracts/Bases/WidgetContext.cs))를 상속하고, 대응하는 직렬화 DTO인 **`WidgetState`** 를 가집니다. `CommunityToolkit.Mvvm` 기반 `ObservableObject` 패턴.

- **Context ↔ State 양방향 바인딩**: `OnCreate(state)` → `OnStateChanged(state)` → (UI 변경) → `SaveState()` → `IAppStateService.UpdateWidget()` → 디스크 JSON 반영.
- **CustomSettings 이중 동기화**: `CustomSettingsContext` ([src/Shared/Corathing.Contracts/Bases/CustomSettingsContext.cs](src/Shared/Corathing.Contracts/Bases/CustomSettingsContext.cs)) 가 `OnContextChanged()` (뷰 → DTO) 와 `OnSettingsChanged(option)` (DTO → 뷰) 두 방향 훅을 제공. 위젯별 옵션(예: `OpenerOption`, `WebPageOption`) 이 `WidgetState.CustomSettings` 에 JSON으로 직렬화됨.
- **DataSource ↔ Widget 연결**: 위젯은 필요 시 `IDataSourceService.GetOrFirstOrCreateDataSourceContext<T>()` 로 공유 DataSourceContext를 가져와 사용 (예: `OpenerWidget` ↔ `ExecutableAppDataSourceContext`, `ToDoListWidget` ↔ `ToDoDataSourceContext`).
- **메시징**: `WeakReferenceMessenger.Default` 로 상태 변경 브로드캐스트 — `WidgetStateChangedMessage`, `DataSourceStateChangedMessage`, `CustomSettingsChangedMessage`, `PackageStateChangedMessage`.

### 새 위젯 추가 방법

위치: [src/Widgets/Corathing.Widgets.Basics/Widgets/](src/Widgets/Corathing.Widgets.Basics/Widgets/) 하위에 폴더 생성.

최소 4개 파일이 필요:
1. `MyWidgetContext.cs` — `WidgetContext` 상속, `[EntryCoraWidget(...)]` + 언어별 `[EntryCoraWidgetName/Description/DefaultTitle(ApplicationLanguage.en_US, ...)]` 속성 부착. 참고 예: [OpenerWidgetContext.cs](src/Widgets/Corathing.Widgets.Basics/Widgets/Openers/OpenerWidgetContext.cs).
2. `MyWidgetOption.cs` — 직렬화 가능한 POCO.
3. `MyWidgetOptionContext.cs` — `CustomSettingsContext` 상속, 설정 UI 바인딩.
4. `MyWidget.xaml` + `.xaml.cs` — 위젯의 시각적 표현.

그 후 빌드만 하면 `PackageService.LoadDLL` 이 리플렉션으로 자동 발견. 수동 등록 코드를 추가할 필요가 없습니다. 로컬라이즈 문자열은 [Resources/BasicWidgetStringResources.resx](src/Widgets/Corathing.Widgets.Basics/Resources/BasicWidgetStringResources.resx) 에 추가.

데이터소스도 같은 패턴 — [src/Widgets/Corathing.Widgets.Basics/DataSources/](src/Widgets/Corathing.Widgets.Basics/DataSources/) 아래에 `DataSourceContext` 상속 + `[EntryCoraDataSource]` 속성.

### 서비스 레이어 (Organizer.WPF DI)

대부분의 서비스 인터페이스는 [src/Shared/Corathing.Contracts/Services/](src/Shared/Corathing.Contracts/Services/) 에 정의되고, WPF 구현체는 [src/Apps/Corathing.Organizer.WPF/Services/](src/Apps/Corathing.Organizer.WPF/Services/) 또는 [src/Apps/Corathing.Organizer/Services/](src/Apps/Corathing.Organizer/Services/) 에 있습니다:

| 인터페이스 | 구현 위치 | 역할 |
|---|---|---|
| `IAppStateService` | Organizer.WPF/Services/AppStateService.cs | 전체 상태 트리 로드/저장, Project/Workflow/Widget/DataSource CRUD |
| `IPackageService` | Organizer/Services/PackageService.cs | DLL/NuGet 로드, 리플렉션 기반 위젯/데이터소스 메타정보 등록 |
| `IDataSourceService` | Organizer/Services/DataSourceService.cs | DataSourceContext 생성/파기/조회 (Guid 기반) |
| `ILocalizationService` | Dashboards.WPF/Services/LocalizationService.cs (싱글턴) | 네임스페이스별 `ResourceManager` 레지스트리 + 언어 전환 이벤트 |
| `IThemeService` | Organizer.WPF/Services/ThemeService.cs | Light/Dark + Mica/Acrylic 적용, ResourceDictionary 스왑 |
| `IStorageService` | Organizer.WPF/Services/StorageService.cs | 글로벌/로컬/커스텀 AppData 경로 및 엔터티별 폴더 관리 |
| `IDialogService` | Organizer.WPF/Services/DialogService.cs | Snackbar 알림 |
| `INavigationDialogService` | Organizer.WPF/Services/NavigationDialogService.cs | 설정 화면용 breadcrumb 모달 |
| `IAuthService` | Organizer.WPF/Services/AuthService.cs | JWT (Supabase stub) |
| `ISecretService` | Organizer.WPF/Services/ModelVersionSecretService.cs | 민감 설정 저장 stub |

### Server / Identity / Database (미완성)

[src/Apps/Corathing.Organizer.Server](src/Apps/Corathing.Organizer.Server/), [Corathing.Organizer.Identity](src/Apps/Corathing.Organizer.Identity/), [Corathing.Organizer.Database](src/Apps/Corathing.Organizer.Database/) 는 향후 원격 동기/인증을 위한 ASP.NET Core 서버 스켈레톤입니다. 현재는 거의 비어 있으며, README의 RSA/JWT user-secrets 지시(`dotnet user-secrets set "Jwt:PublicKey"`)는 Server 프로젝트에만 필요하고 WPF 앱 자체 실행에는 **불필요**합니다.

### 로컬라이제이션 규약

- 지원 언어: `ApplicationLanguage.en_US`, `ApplicationLanguage.ko_KR` (+ `Unknown` 폴백).
- 각 어셈블리는 자기 `.resx` 를 들고 있다가 부팅 시 `LocalizationService.RegisterStringResourceManager(namespace, ResourceManager)` 로 등록.
- XAML에서 사용: `{localization:Localization Key}` (마크업 확장, [src/Shared/Corathing.Dashboards.WPF/Bindings/LocalizationExtension.cs](src/Shared/Corathing.Dashboards.WPF/Bindings/LocalizationExtension.cs)).
- 위젯 속성에서 언어별 텍스트는 `[EntryCoraWidgetName(ApplicationLanguage.ko_KR, "열기")]` 형태로 선언.
- `.resx` 접근자 클래스([BasicWidgetStringResources.cs](src/Widgets/Corathing.Widgets.Basics/Resources/BasicWidgetStringResources.cs), [CorathingOrganizerLocalizationStringResources.cs](src/Apps/Corathing.Organizer.WPF/Resources/CorathingOrganizerLocalizationStringResources.cs)) 는 **수동 작성된 얇은 wrapper** 입니다. 과거에 쓰던 `PublicResXFileCodeGenerator` + `*.Designer.cs` 자동 생성은 사용하지 않습니다. `.resx` 에 키를 추가해도 wrapper 는 바뀌지 않으므로 `git update-index --assume-unchanged` 같은 관습도 필요 없습니다.

### 상태 영속화

- 기본 경로: `%APPDATA%\Corathing\AppData\Current\cora-organizer-settings.json` (글로벌) 또는 앱 실행 폴더의 `AppData\Current\` (로컬).
- JSON 스키마: `{ Preferences, Packages, Dashboards: { Projects, Workflows, Widgets, DataSources } }`.
- `AppStateService` 가 메모리 캐시를 유지하고 명시적 `Flush` 혹은 변경 즉시 저장 (구현 확인 필요).
- 최초 실행 템플릿은 [AppData/Current/cora-organizer-settings.json](src/Apps/Corathing.Organizer.WPF/AppData/Current/cora-organizer-settings.json) 에 포함되어 출력 폴더에 복사됨.

## 로드맵 / 미완성 항목

상세는 [docs/todo.md](docs/todo.md), [docs/roadmap.md](docs/roadmap.md) 참고. 요약:

- `DataSourceContext.OnMessage` / `WidgetContext.OnMessage` 구현 — 위젯 간 메시지 전달.
- `NavigationService` (Usa.Smart.Navigation 기반) 통합.
- 대시보드 `TabControl` 의 Add 버튼 재구현.
- 위젯 크기 프리셋, 배경색 변경, 커스텀 아이콘.
- 커스텀 설정 파일 로드, 게스트 모드, 자동 패키지 로드.
- 단위테스트 프로젝트 추가 (`dotnet test` CI 단계).

## 추가 참고

- 아키텍처 / 위젯 카탈로그 상세: [docs/memory/주요요소.md](docs/memory/주요요소.md)
- 기여 가이드: [CONTRIBUTE.md](CONTRIBUTE.md)
- 버전: [version.json](version.json) (Nerdbank.GitVersioning, 현재 `1.1`)
