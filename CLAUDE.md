# CLAUDE.md

이 저장소는 **PunkerArena** (원격: `git@github.com:ginojang/PunkerArena.git`) 입니다.
작업 폴더 `FruttiDino_RPG/`는 `.gitignore`로 **버전 관리에서 제외**되어 있습니다 (아래는 분석 기록용).

---

## FruttiDino_RPG (후르츠디노 RPG) 개요

턴제 웨이브 전투 기반 모바일 RPG. **Unity 클라이언트 + .NET 6 게임 서버** 구성이며,
클라이언트–서버 실시간 통신은 **MagicOnion(gRPC)** + **MessagePack**, 로비 통신은 **Protobuf(gRPC)**를 사용한다.
데이터 저장은 **MySQL**(MySqlConnector) + **Redis**(StackExchange.Redis, MagicOnion Group Repository).

### 최상위 폴더 구조

| 폴더 | 설명 |
|------|------|
| `Server/` | **메인 게임 서버** (.NET 6, `net6.0`). MagicOnion 서버 + gRPC. 실제 운영 대상. |
| `Shared/` | 서버/콘솔 공용 프로젝트 (`Shared.csproj`, 인터페이스/메시지 정의). |
| `Client/` | **Unity 클라이언트** (약 1,220 cs). MagicOnion.Unity, Addressables, Spine, DOTween, TextMeshPro. |
| `Project_FruitDino/` | 구버전/원본 Unity 클라이언트 프로젝트 (약 1,329 cs). Client와 별개 계보로 보임. |
| `FruttiDinoRPGConsol/` | Unity 기반 **콘솔 전투 테스트** 프로젝트 (약 252 cs). |
| `FruttiDinoRPGConsolServer/` | 콘솔용 서버 (Server/Shared 재구성, 24 cs). |
| `FruttiDinoRPGTest/` | Unity 테스트 프로젝트 (17 cs). |
| `Table/` | 게임 밸런스 원본 테이블 `.xlsm` + 추출된 `Table/csv/` (27개 CSV). |
| `GameDesign_Doc/` | 기획 문서 (전투/스킬/몬스터/캐릭터/스탯/UI/애니메이션, 로드맵 xlsx). |
| `GameDevelop_Doc/` | 개발 문서 (서버규격서, 콘솔상세스팩, MagicOnion 설치법, AI 설계 등 docx/pptx/pdf). |

---

## 서버 (`Server/`) — 핵심

- **엔트리**: `Server/Entry/Program.cs`
  - `WebApplication` + `AddGrpc()` + `AddMagicOnion()`, `MapMagicOnionService()`.
  - `SystemBoot.StartSystem()`에서 싱글턴 부팅.
  - Redis는 `#define USE_REDIS` 조건부 컴파일(기본 비활성).
- **부팅**: `Server/Entry/SystemBoot.cs`
  - 싱글턴 생성: `GameControlManager`, `WaveHubManager`, `GrpcConnect`, `RedisManagement`.
  - `TestTableDataManager`로 임시 DB 접속 및 Dino Body/Part 테이블 초기화.
- **설정**: `Server/Entry/AppSetting.cs` (`appsettings.json` 로더, `Singleton<AppSetting>`), `appsettings.json` / `appsettings.Development.json`.
- **배포**: `Server/Dockerfile` (Linux 타겟).

### 통신 계층 (`Server/MagicOnion/`)
- `Hubs/Battle/WaveCosolBattleHub.cs`, `WaveHubManager.cs` — 실시간 웨이브 전투 StreamingHub.
- `Hubs/Battle/TestEchoHub.cs` — 연결 테스트용.
- `Services/ServerGameControl.cs`, `Services/TableReqeust.cs` — Unary 서비스(게임 제어, 테이블 요청).

### 전투 로직 (`Server/Main/Game/Battle/`)
- **`ServerWaveCore/`** — 현행 서버 권위(server-authoritative) 웨이브 전투 코어.
  - `AICore/` — `AICore`, `AICoreFactory`, `NormalMonsterAICore`, `BossMonsterAICore`, `UserAICore`.
  - `WaveStatus/` — 상태 머신(`WaveStatusMachineManager` + 다수 상태: WaveStart/TurnStart/ActionOrder/ProcessSide(0/1/PVP)/ShowCC/WaveFinish 등).
  - `Interaction/` — `Interaction`, `InteractionNormalFight`, `SkillContainer`, `TriggerManager` (스킬/트리거 처리).
  - `Data/` — `DataCoreWave`, `DataCoreBattleCommand`.
  - `Interface/IWaveCore.cs`.
- **`Battle_Legacy/`** — 구버전 전투(Player/Unit/Dino/Wave). 참고용 레거시.
- `TestMagicOnion/`, `TestS3orLobbyServer/` — 테스트 스캐폴딩(`TestTableDataManager`, `TestMortalCreator`).

### 서버 의존성 (`Server/Server.csproj`)
- `MagicOnion.Server` 4.5.2, `MagicOnion.Server.Redis` 4.5.2
- `Grpc.AspNetCore` 2.40.0, `Google.Protobuf` 3.21.9, `Grpc.Core` 2.46.5, `Grpc.Tools` 2.50.0
- `MySqlConnector` 2.3.0-beta.1
- `Protos/*.proto`는 **GrpcServices="Client"** 로 생성, `..\Shared\Shared.csproj` 참조.

---

## 프로토콜 (`Server/Protos/`)
- `common.proto` (package `common`) — 공용 enum. `CommonErrorCode`(ERR_SUCCESS=0, 세션 만료/중복 등), `CommonStoreType`(1 AOS, 2 iOS, 3 OneStore).
- `lobby.proto` (package `lobby`) — game client ↔ **lobby server** 통신.
  - 네이밍 규약: postfix `Request`/`Response`/`Noti`(서버 push)/`Report`(단방향), prefix `list_`, `_count`/`_info`.
  - 공통 필드: `sid`(세션, 유효 1시간·요청마다 연장, metadata 전송), `result`, `store_type`, `language_type`(10 영어/23 한국어/22 일본어/6 중국어/34 스페인어).
  - `option java_package = "com.fruttidino.proto"`.
- **주의**: 로비(Protobuf gRPC)와 인게임 전투(MagicOnion Hub)는 서로 다른 통신 경로.

---

## 클라이언트 (`Client/`) — Unity
주요 `Assets/` 영역: `Contents/`(Scenes, Scripts — 게임 로직), `Framework/`, `GUI/`, `Asset/GameData`,
`Res_InBuild`·`Res_InPatch`(Addressables 빌드/패치 리소스), `3rdParty/`(MagicOnion, MessagePack),
`Extern/`(Demigiant=DOTween, StateMachine, StompyRobot=SRDebugger), `Spine`, `TextMesh Pro`, `RealToon`.
- gRPC 코드 생성: `Client/Protobuild.bat` + `protoc.exe` + `grpc_csharp_plugin.exe`, `Client/Protos/`.

---

## 데이터 테이블 (`Table/`)
- 원본: `Table/*.xlsm` (Dino/Item/Language/Monster/Pos_Card/Reward/Skill/Stage 등).
- 추출본: `Table/csv/*.csv` (27개) — 예: `ChapterTBL`, `DinoBaseTBL`, `DinoPartsTBL`, `DinoLevelTBL`,
  `DinoStatGrowthRankTBL`, `DinoRoleTBL`, `DinoAttributeTBL`, `ConditionTBL`, `GuideTBL` 등.
- 게임 핵심 소재는 **Dino(공룡) 파츠/스탯/성장** 중심.

---

## 개발 참고
- 솔루션: `Server/Server.sln`, `Shared/Shared.sln`, `FruttiDinoRPGConsol/FruttiDinoRPGConsol.sln`.
- 서버 빌드/실행: `Server/` 에서 `dotnet build` / `dotnet run` (net6.0). Docker는 `Server/Dockerfile`.
- 코드/주석에 한국어가 많고, 일부 네임스페이스가 `k514`, `s0361` 등 코드명으로 되어 있음.
- **레거시 주의**: `Battle_Legacy`(서버), `Project_FruitDino`(구 클라이언트)는 현행이 아닐 수 있으므로
  현행 전투 로직은 `Server/Main/Game/Battle/ServerWaveCore/`, 현행 클라이언트는 `Client/`를 우선 참조.

---

## 기술 결정 기록 (ADR)

### ADR-001: 실시간 통신에서 MagicOnion 포기 (2026-07-26)

**상태: 결정됨 — MagicOnion 폐기 / 대안은 미정(검토 중)**

- **결정**: 인게임 실시간 통신 스택으로 **MagicOnion을 더 이상 사용하지 않는다.**
- **이유(핵심)**: **Unity WebGL 지원이 필수**인데, MagicOnion의 기본 전송(gRPC over HTTP/2)은
  브라우저 환경(WebGL)에서 동작하지 않는다. 브라우저는 raw HTTP/2 gRPC 스트리밍 소켓을 열 수 없다.
- **보조 이유**: 현재 프로젝트가 `MagicOnion.Server 4.5.2` + **.NET 6**(EOL)로 3세대 뒤처져 있어,
  유지하려면 어차피 `.NET 8 + MagicOnion 7` 대규모 업그레이드가 선행되어야 한다.

**참고 — MagicOnion으로 WebGL이 불가능한 것은 아님(하지만 채택 안 함):**
- Cysharp의 `GrpcWebSocketBridge`(gRPC over WebSocket/HTTP1)로 우회 가능.
  단 **MagicOnion 7 + .NET 8 + Unity 6.3+** 필요, `WebGLThreadDispatcher` 등 세팅 까다롭고 알려진 이슈 존재.
- 업그레이드 비용 + 브릿지 불안정성을 감수할 이유가 약해 **포기 결정**.

**대안 후보 (아직 확정 안 됨):**
| 후보 | WebGL | 기존 C# 서버 전투코어 재활용 | 메모 |
|------|:---:|:---:|------|
| **SignalR** (유력) | ✅ 네이티브 WebSocket | ✅ .NET 서버 유지, StreamingHub↔Hub 개념 유사 | 통신 계층만 교체, MessagePack 프로토콜 지원 |
| Nakama | ✅ | ❌ Go+Lua/JS, 전투 재작성 | 매칭/소셜 필요 시 |
| Photon Fusion/Quantum | ✅ | ❌ | 실시간 액션 특화, 턴제엔 과함 |
| Colyseus | ✅ | ❌ Node/TS | 서버 전면 재작성 |

**보존해야 할 자산(대안 선정 기준)**: 서버권위 전투 로직 `Server/Main/Game/Battle/ServerWaveCore/`
(`WaveStatus` 상태머신, `AICore`, `Interaction/SkillContainer`). 이 C# 코어 재활용도가 대안 선택의 1순위 기준.

**미결 사항(TODO)**: ① 대안 최종 확정(SignalR 유력) ② 로비(Protobuf gRPC)의 WebGL 호환성 재검토
③ `.NET 8` 정렬 ④ Unity 클라 통신 레이어 교체 범위 산정.
