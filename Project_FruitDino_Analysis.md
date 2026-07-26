# Project_FruitDino 상세 분석

> 분석 대상: `FruttiDino_RPG/Project_FruitDino/` (gitignore 대상 폴더 — 분석 기록용)
> 최초 작성: 2026-07-26 · 관련: [CLAUDE.md](CLAUDE.md) 의 프로젝트 개요·ADR-001

## 정체성 (한 줄 요약)

> ⚠️ **중요 정정 (2026-07-26)**: Project_FruitDino는 현행 `Server/`+`Client/`(후르츠디노 RPG)의
> "구버전"이 **아니다**. **전혀 다른 게임**이다. 따라서 현행 서버 전투코어(`ServerWaveCore`)와
> **서버 로직을 공유할 수 없으며**, 이 게임의 **전투 판정은 클라이언트에 그대로 유지되어야 한다.**

**독립적인 다른 게임의 Unity 클라이언트.** **전투가 클라이언트에서 실행되는 client-authoritative 구조**로,
전투 판정·계산이 클라 내부에서 자기완결적으로 처리된다(서버 권위 아님).

- 타겟: **.NET Framework 4.7.1** (`net471`, `packages.config` 기준)
- 형태: Unity 프로젝트 (`.csproj`는 Unity가 생성 → 저장 안 됨)
- FSM 라이브러리: `MonsterLove.StateMachine`
- 시기 흔적: 2022-10 (`Protobuild.bat`, `UpgradeLog.htm`)
- 실제 게임 코드: `Assets/Scripts/` 하위 **약 235개 `.cs`** (나머지 1,300여 개는 에셋스토어 플러그인)

---

## 아키텍처 레이어별 분석

### 1. 네트워크 (`Assets/Scripts/Network/`, `Manager/NetworkManager.cs`, `Packet/`)
- **raw gRPC 사용** (`Grpc.Core.Channel`) — ⚠️ MagicOnion 아님. 로비 통신만 **Protobuf Unary**.
- `ClientNetworkContents.cs`: `SendPacket<TResponse>()` 공통 래퍼 + `CheckError(RpcException)`
  (에러 상세를 `|` 구분자로 파싱: `errorCode | detail`).
- `ClientNetworkContents_Lobby.cs`: `SendLogin`(SigninService), `SendUsereData`(GetUserDataService),
  `SendSetCharacter`(DevSetDinoService) 등 Unary 호출.
- `Packet/`: `common.proto`/`lobby.proto`에서 생성된 C# (`Common.cs`, `Lobby.cs`, `LobbyGrpc.cs`).
- **⚠️ WebGL 불가 확정 근거**: `Grpc.Core`는 네이티브 라이브러리 기반 → **WebGL 빌드 불가**.
  이 구버전 클라도 WebGL 못 씀 (→ ADR-001 결정을 뒷받침).

### 2. 데이터 호스팅 (`Network/AwsConnect.cs`)
- **AWS S3** 버킷 `futtidino-data`(경로 `game-data`)에서 `ServerInfoData.json` 등 다운로드.
- **AWS Cognito**(Identity Pool)로 인증. 리전 `ap-southeast-1`(싱가포르).

### 3. 전투 시스템 — 핵심 (`Manager/InGameManager/`)
- **`BattleManager`** = 14단계 FSM (`enum TRIGGER_FSM`):
  `Load → Intro → TurnReady → InitTurn → SkillSelect → CharacterAction → OnStack → OnBurst
  → ActionEnd → SetRoundUI → InitData → TurnEnd → GameResult`
  - 각 상태를 partial 파일로 분리: `BattleManagerFSM/BattleManager_<State>.cs` (`_Enter/_Update/_Exit`).
  - 진행 방식: `stateAction` 액션 큐 + `Messenger.Broadcast(...)` 이벤트 버스.
- **`OnStack` / `OnBurst`**: 스택/버스트 기반 전투 메커니즘 (게임 고유 시스템).
- 서브시스템 매니저: `BuffManager`, `CCManager`(상태이상), `EffectManager`, `GridManager`,
  `StackManager`, `TriggerManager`, `TurnManager`.
- **`System/`**: `CalculateSystem`(데미지 계산 — **현재 대부분 주석 처리, 미완/리팩터링 중**),
  `TimeSystem`, `TriggerSystem`, `Grid`, `Turn`, `SkillButton`, `TimeColor`, `TImeObject`(오타).
- **`AIManager`**: 클라이언트 측 몬스터 AI.

### 4. 캐릭터 / 몬스터 (`Character/`, `Monster/`)
- 각각 **자체 서브 FSM** 보유 (`SubState/`):
  `ActionStart / ActionEnd / Wait / Death / OnDamage / MeleeAction /
   RangeActionDirect / RangeActionParabola / RunMove(Forward|Back|Center)`.
- 애니메이션 연동 행동 상태머신 → 근접/원거리(직선·포물선) 공격, 이동, 피격, 사망 연출.
- 구조: `CharacterBase` 상속, `Character` + `Character_SubAction`(partial). Monster도 대칭 구조.

### 5. 데이터 파이프라인 (`CSV/`, `Manager/CSVDataManager.cs`)
- **46개 테이블 클래스 자동 생성** (`Generated.CsvData` 네임스페이스): 예)
  `DinoTable`, `DinoStatusTable`, `Dino_ClassTable`, `MonsterTable`, `MonsterAITable`,
  `Monster_GroupTable`, `Monster_PartTable`, `BuffTable`, `BurstTable`, `BurstTypeTable`,
  `AttributeTable`, `EffectTable`/`EffectListTable`, `ExpTable`, `ItemTable`,
  `RandomOptionTable`, `SetOptionTable`, `PureDnaBonusTable`, `LimitValueTable`,
  `AniTable`/`AniSkillTable`, `Anim_EventTable`, `CameraTable`, `ConditionTable`,
  `ConstBaseTable`, `DefTable`, `ArgTable`, `HelpPopupBaseTable` 등.
- 파서: `CSVReader`, `CSVTableHepler`(오타 포함).
- 원본 워크플로우: `table/DinoTable_*.xlsm` → **NPOI**(`packages.config`)로 CSV 추출.
- `ScriptableData/`: `DinoPartsData`, `DinoPartsBaseData` (ScriptableObject).
  `PartsData/CharacterPartData`.

### 6. 프레임워크 (`Framework/`)
- **`Base/`**: `Singleton`, `SingletonWithMonoBehaviour`, **`Messenger`**(이벤트 버스 — 전투 흐름의 중추),
  `ImmortalGameObject`(DontDestroyOnLoad), `MultiDictionary`, `Utility`, 커스텀 `Debug`,
  `LiteButton`/`LiteText`, `GeoProperty`.
- **`Asset/`**: Unity **Addressables** 로딩 + **오브젝트 풀링**
  (`UiAddressablePoolManager`, `PoolController`, `AddressableAssetDownloadLoader`, `AssetBundleLoader`),
  `AssetManager`/`AssetLoader`/`AssetMapper`.
- **`Sound/`**(`Sound`, `SoundFadeEvent`), **`UnitySceneLoader/`**, **`Event/`**(`Messenger`, `Callback`).

### 7. UI / 씬 (`Contents/`, `GUI/`, `Scenes/`)
- `Contents/`: `UiTitle`, `UiMenu`, `UiGame`, `UiPopup`, `UiPatch`(패치),
  `UiLoadingBasic`, `UiHelpPopup`, `UiScreenTransiton`.
- `Scenes/Loader`, `Scenes/UiController`.
- 비주얼 에셋: GUI PRO Kit, RealToon(툰 셰이딩), AmplifyShaderEditor, Boing Kit(젤리 물리), KTK Effect.

---

## 현행 후르츠디노 RPG(Server/ + Client/)와의 관계

**둘은 서로 다른 게임이다.** 아래는 "같은 게임의 구/신 버전" 대조가 **아니라**, 별개 프로젝트의
아키텍처 성격 비교다. 특히 전투 권위 모델이 근본적으로 달라 **서버 로직을 공유할 수 없다.**

| 항목 | Project_FruitDino (별개 게임) | 후르츠디노 RPG `Server/`+`Client/` |
|------|------|------|
| 전투 권위 | **클라이언트** (`BattleManager` FSM, 판정 클라 내장) | **서버** (`ServerWaveCore`) |
| 실시간 통신 | raw gRPC (로비 Unary만) | MagicOnion StreamingHub |
| 타겟 프레임워크 | .NET Framework 4.7.1 | .NET 6 |
| 데이터 | CSV(로컬 Resources / 디바이스는 S3) | 유사 CSV + 서버 테이블 |
| 코드 공유 | ❌ **서버 전투코어 공유 불가** — 클라 판정 유지 | — |

---

## 실행 / AWS 의존성 (오프라인 동작 확인)

부팅 흐름 `Scenes/Main.cs` → `OnAssetManagerIntializeComplete()`에서 플랫폼별로 갈린다:

| 환경 | 테이블 로딩 | AWS 필요? |
|------|------|:---:|
| **Editor / Windows / OSX** | `CSVDataManager.InitTables()` → 로컬 `Resources/CSV/*.csv`(24개) | ❌ 불필요 |
| **Android(디바이스)** | `CSVDataManager.InitAWSData()` → S3 `game-data` | ✅ 필요 |

- `InitAWSData()`는 `InitTables()` 안에서 **주석 처리**됨. 에디터 경로는 전적으로 로컬 CSV 사용.
- **전투는 client-authoritative + 데이터 로컬** → 전투 자체는 서버/AWS 없이 클라에서 완결.
- **막히는 지점 2곳**:
  1. 타이틀 로그인 `TitleUiController.SendLogin` → **EC2 gRPC 로비서버**(`ec2-13-212-...:6565`).
     실패 시 `response == null` → **`Application.Quit()`**.
  2. 디바이스 빌드는 테이블을 S3에서 받으므로 AWS 필수.
- **결론**: 에디터에서 로그인/`Quit()` 우회 또는 배틀 씬(`GameScene`/`main`/`Scenes/Tool/*`)을 직접 열면
  **AWS 없이 전투 확인 가능**. 정상 부팅 플로우는 EC2 로그인에서 종료되므로 그대로는 안 됨.
  → 클라 판정이 로컬·자기완결적이라 "클라 판정 그대로 유지" 방향과 구조가 부합.

---

## 전략적 시사점

1. **서버 로직 공유 불가**: 현행 후르츠디노 RPG와 **다른 게임**이므로 `ServerWaveCore` 등 서버 전투코어를
   재사용할 수 없다. **전투 판정은 이 클라이언트 안에 그대로 유지**해야 한다(제거·서버 이관 대상 아님).
2. **WebGL 관점**: `Grpc.Core`(네이티브) 때문에 이 클라도 WebGL 불가 → WebGL이 필요하면 통신/로그인
   레이어(로비 gRPC) 교체가 선행돼야 함. 단 **전투 판정 자체는 클라 로컬이라 WebGL 이식과 무관하게 보존** 가능.
3. **재활용 가치 있는 자산**: 전투 연출/애니메이션 FSM(`Character/Monster/SubState`), 전투 판정/계산
   (`BattleManager` FSM, `CalculateSystem` 등), UI(`Contents`), CSV 파이프라인, 풀링/Addressable
   프레임워크(`Framework/Asset`) — 이 게임의 핵심 자산으로 유지.

## ⚠️ 보안 이슈 (조치 필요)

- `Network/AwsConnect.cs`: **AWS 액세스 키가 주석 형태로 소스에 잔존**, **Cognito Identity Pool ID 하드코딩**.
- `Manager/NetworkManager.cs`: 게임 서버 주소 하드코딩
  (`ec2-13-212-207-205.ap-southeast-1.compute.amazonaws.com:6565`).
- **권장**: 노출 가능성이 있는 키는 **즉시 회수(rotate)**, 서버 주소·풀 ID는 설정/환경 분리.
