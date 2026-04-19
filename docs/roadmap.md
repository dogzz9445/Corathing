# Roadmap — 버전 마일스톤

> 하이레벨 큰 그림. 구체적 작업은 [todo.md](todo.md), 아이디어 백로그는 [milestones.md](milestones.md).

---

## 1.0 — 안정화 (현재)

**목표:** 개인 사용으로 안정적으로 돌아가는 위젯 대시보드.

- 핵심 기능 완성 — [todo.md](todo.md) 항목 전부
- 위젯 간 메시징 (`OnMessage`)
- Settings 화면 안정화 (TabControl Add, Generator Key 기반 설정)
- 단위테스트 + CI `dotnet test` 단계
- GitHub Release 자동화 (git tag → self-contained zip, **이미 구현**)

## 2.0 — 확장

**목표:** "내 것" 에서 "공유 가능한 것" 으로.

- NavigationService 기반 위젯→위젯 팝업/모달
- 사용자 지정 설정 파일 불러오기/저장
- 게스트 모드 (설정 암호화)
- 자동 패키지 로드 (지정 폴더 감시)
- ChatGPT / 소켓 / 파이썬 / 차트 / 동영상 등 위젯 확장 (→ [milestones.md](milestones.md))
- 토스트 메시지, 핑크 테마 등 편의 기능

## 3.0 — 커뮤니티 / 배포

**목표:** 써드파티 위젯 생태계.

- `Corathing.Contracts` API 확정 (SemVer 정책)
- NuGet 패키지 구성 스펙 확정 (메타데이터, 아이콘, 다국어)
- 위젯 패키지 저장소 설계 (공식/커뮤니티 피드)
- 패키지 관리 UI 고도화 (검색, 설치, 업데이트, 제거)
- 홈페이지 / 문서 사이트 / NuGet.org 배포
- 게스트 모드 배포 방식 설계 (엔터프라이즈 대응)
