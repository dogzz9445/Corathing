# Corathing <img src="docs/images/logo_256.png" alt="drawing" width="32"/> 
### Organize Anything with Customizable Widget Dashboards 

Corathing is Customizable Widget Organizer. It is a WPF application that allows you to organize anything with customizable widget dashboards. You can create your own widgets or use the default widgets provided by Corathing.

| Homepage |  Wiki |Readme | Readme |
| --- | --- | --- | --- |
|  [Homepage](https://corathing.com) | [Wiki](https://github.com/dogzz9445/Corathing/wiki/Home) | [English](README.md) | [한국어](README_KR.md) | 

![sample](docs/images/version0.0.9.gif)

## 01. Inspiration (영감)
- [Freeter](https://github.com/FreeterApp/Freeter) - Freeter is a productivity app that allows you to gather everything in one place.
  - UI/UX 및 기능들에 영감을 얻었습니다.
- [Grafana](https://grafana.com/) - Grafana is the open-source analytics & monitoring solution for every database.
  - 대시보드 모니터링에 영감을 얻었습니다.

## 02. Features

✔️ Features
-
- Dashboard host and widget host implementation and bug fixes, testing

❌ Features - TBD
- 

## 03. Project Structure

📁 Shared
-
| Name| Folder |Framework | Description | Version
| --- | --- | --- | --- | --- |
| Corathing.Contracts | src/Shared | .Net 8.0 |  | ```진행중```
| Corathing.Contracts.Utils | src/Shared | .Net 8.0 |  | ```진행중```
| Corathing.Dashboards | src/Shared | .Net 8.0 |  | ```진행중```
| Corathing.Dashboards.WPF | src/Shared | WPF |  | ```진행중```
| Corathing.UI | src/Shared | .Net 8.0 |  | ```진행중```
| Corathing.UI.WPF | src/Shared | WPF |  | ```진행중```


📁 Apps
-
| Name| Folder |Framework | Description | Version
| --- | --- | --- | --- | --- |
| Corathing.Organizer | src/Apps | .Net 8.0 |  | ```진행중```
| Corathing.Organizer.WPF | src/Apps | WPF |  | ```진행중```


📁 Widgets
-
| Name| Folder |Framework | Description | Version
| --- | --- | --- | --- | --- |
| [Corathing.Widgets.Basics](src/Widgets/Corathing.Widgets.Basics/README.md) | src/Widgets | WPF |  | ```진행중```
|  |  | WPF |  | ```시작전```

📁 UnitTests
-
| Name| Folder |Framework | Description | Version
| --- | --- | --- | --- | --- |
|  |  | WPF |  | ```시작전```

## 04. Getting Started

### 04.01. Prerequisites

- RSA Key Generation
  - You need to generate RSA keys for JWT authentication. You can use the following command to generate the keys:
  - Generation Sample: https://cryptotools.net/rsagen
```bash
cd src/Apps/Corathing.Organizer.Server
dotnet user-secrets init
dotnet user-secrets set "Jwt:PublicKey" "your-public-key-value"
dotnet user-secrets set "Jwt:PrivateKey" "your-secret-key-value"
```


## 05. Packages

📕 library to use
-

| Name | Where to use | Version |
| --- | --- | --- |
| [Microsoft.EntityFrameworkCore](https://learn.microsoft.com/ko-kr/ef/core/) | |
| [Microsoft.Extensions.Logger](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging) | |
| [Microsoft.Extensions.Configuration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration) | |
| [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) | |
| [Microsoft.Extensions.Localization](https://learn.microsoft.com/en-us/dotnet/core/extensions/localization) | |
| [Microsoft.Xaml.Behaviors.Wpf](https://github.com/microsoft/XamlBehaviorsWpf) | |
| [Microsoft.CommunityToolkit.MVVM](https://learn.microsoft.com/ko-kr/dotnet/communitytoolkit/mvvm/) | |
| [Microsoft.Toolkit.WebView](https://learn.microsoft.com/en-us/windows/communitytoolkit/controls/wpf-winforms/webview) | |

- Virtual Serial Port (IO Test용)
- LiveLogViewer (디버그용, 수정해서 사용 중)
- [Prometheus-net MIT] (https://github.com/prometheus-net/prometheus-net)

### THIRD_PARTY UI
- [MaterialDesign](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
    - MaterialDesign.Icons
    - MaterialDesignColors
    - MaterialDesignThemes
    - MaterialDesignThemes.MahApps
- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)


## 06. Features

### TODO
- [ ] 인터페이스로 DataSource 나 Widget 으로부터 인터페이스(API 느낌의?? ENI, ENtity Interface?) 를 등록하거나 호출 가능하도록 구조 변경
