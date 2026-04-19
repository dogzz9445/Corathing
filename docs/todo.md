# TODO — 현재 안 되는 핵심 기능

> 순서대로 하나씩 개발한다. 위에서 아래로.
> 위젯 아이디어 · 편의 기능은 [milestones.md](milestones.md), 버전 계획은 [roadmap.md](roadmap.md) 참고.

---

## 1. 위젯 간 메시지 전달 (WidgetSystem)

- [ ] `DataSourceContext.OnMessage` 구현 — 위젯 → 데이터소스 → 위젯 브로드캐스트
- [ ] `WidgetContext.OnMessage` 구현 — 위젯 간 직접 메시지
- [ ] 메시지 스키마 정의 (Contracts/Messages/)
- [ ] 샘플: Opener 위젯이 ExecutableApp DataSource 변경을 수신해서 UI 갱신

## 2. Settings 화면 안정화 (Organizer)

- [ ] Dashboard View `TabControl` 의 Add 버튼 재구현 — Placeholder 스타일 + 클릭 처리
- [ ] Settings 에서 TypeName 대신 **Generator Key** 사용
  - [ ] `[AssemblyCoraPackageIconAttribute]` 추가
  - [ ] `CoraWidgetGenerator` / `CoraDataSourceGenerator` 가 Key 를 패키지에서 읽도록 변경
- [ ] `ArgumentCastNullException` 추가 (Contracts)

## 3. State/Context 생명주기 정리

- [ ] `ProjectContext`, `WorkflowContext` 생성 메소드 추가
- [ ] `ProjectState`, `WorkflowState`, `WidgetState` 에 `Copy()` 메소드 추가
- [ ] `WidgetContext.Update` 에서 AppState 직접 업데이트 제거 → `WidgetState` 만 갱신
- [ ] `IAppStateService` 가 상태 변경을 감지해서 저장하도록 재배선

## 4. NavigationService 통합 (Organizer)

- [ ] `Usa.Smart.Navigation` 기반 `INavigationService` 구현
- [ ] 설정 화면 breadcrumb 을 `NavigationDialogService` 에서 분리
- [ ] 위젯에서 모달/팝업을 띄울 수 있는 API 제공

## 5. 품질

- [ ] 단위테스트 프로젝트 추가 (`Corathing.Tests`, xUnit)
- [ ] CI 에 `dotnet test` 단계 추가
- [ ] 핵심 서비스(`AppStateService`, `PackageService`, `DataSourceService`) 테스트 커버

---

## Done

최근 완료(참고용). 오래된 것은 git log 참고.

- [x] Dashboard host / Widget host 구현
- [x] Localization / Theme 서비스
- [x] WidgetContext / DataSourceContext
- [x] DLL / NuGet 패키지 로드
- [x] 열기 위젯 (파일/폴더/링크)
- [x] Avalonia 레거시 참조 제거 (2026-04)
- [x] .NET 10 + CI/CD 단순화 (2026-04)
- [x] Localization Designer.cs → 수동 wrapper 전환 (2026-04)
