# 서버 전투 코어 레퍼런스 — "진짜 규칙" (오프라인 클라 이식용)

> FruttiDino RPG 콘솔 서버/테스트 분석 결과. **서버가 하던 진짜 전투 계산**(ArenaBattle 클라가 빠뜨려 임시 `atk−def`로 때운 그 로직)의 실제 공식·스탯·규칙.
> 분석일 2026-07-28. 대상: `FruttiDino_RPG/`(git-ignored, 분석용).

---

## 0. 콘솔 프로젝트의 실체 (뭐가 진짜인지)

| 위치 | 정체 |
|---|---|
| `FruttiDinoRPGConsolServer/Server/Main/Battle/` | ❌ **스텁**. `CalculateDamage()` 비어있음, AttackDino/DefenceDino/SpeedDino **빈 클래스**, 승리조건 "플레이어 2명 미만". 이름만 있는 골격 → **여기서 이식 금지.** |
| `FruttiDinoRPGConsol/` (Unity "콘솔" 클라) | **인-프로세스 전투 놀이터**. 서버 전투코어 `ServerWaveCore/`가 클라 안에 순수 C#으로 들어있고 `Update()`로 구동. MagicOnion은 껍데기(가짜 loopback). **사람 입력식**(오토런 아님), 실서버 MySQL(하드코딩)에서 테이블 로드. |
| **`Server/Main/Game/Battle/ServerWaveCore/`** (메인 서버) | ✅ **진짜 완성된 서버권위 전투 시뮬**. 데미지/크리/회피/관통/방어/속성/턴순서/웨이브/승패/스킬·트리거 다 있음. **이게 레퍼런스.** (동일 사본이 콘솔 클라 `Assets/Scripts/ServerWaveCore/`에도 존재) |

**핵심 파일**: `ServerWaveCore/Interaction/InteractionNormalFight.cs` (데미지), `WaveCore/Data/TableData.cs`의 `Stat`(상수), `IWaveCore.cs`의 `DataDinoCore`(스탯).

---

## 1. 진짜 데미지 공식 (★ 클라가 빠뜨린 것)

**방어 완화 곡선 방식 (%기반, 뺄셈 아님)** — `InteractionNormalFight.cs:268-281`:
```
base   = ATK * ( 50 / (DEF + 50) )            // DAMAGE_VALUE = 50
damage = Random( base*15/16 , base*17/16 )    // ±6.25% 분산
if (ATK < DEF)  damage = 1                     // 방어 > 공격이면 칩 데미지 1
```
> ArenaBattle 임시 `max(1, atk−def)`를 **이 곡선으로 교체**하면 됨. atk10/def10 예: `10*(50/60)=8.3` (지금은 1). 스탯 더미값에도 의미있는 데미지가 나온다.

**크리티컬** (`:286-305`):
```
crit  = base + DEF * (공격자레벨 / 방어자레벨) * 0.5
crit += base * (valueCriticalDamage / 100)     // 크리댐 스탯 % 보너스
```

**방어(Defence)모드 대상 완화** (`GetDefenceDamageRate :356-401`):
```
if ATK >= DEF:  rate = -3*sqrt(ATK-DEF) + 91
else:           rate =  2*sqrt(DEF-ATK) + 84
rate = clamp(0,100);  최종 = damage * (1 - rate/100)
```
→ 방어모드는 **84~91%+ 경감** = 매우 강함.

**공격 판정 순서** (`NormalAttackTo :406-538`):
1. **회피**(`isAvoidSuccess`) → 성공 시 데미지 0.
2. 대상이 방어중이면 **관통**(`isPenetrateSuccess`) 판정. (현재 관통은 최종 ×0.1/×0.05로 오히려 감소 = TODO/미완/버그)
3. 아니면 **크리티컬** 판정 → 크리 vs 일반.
4. **속성 승리 시 최종 데미지 +30%** (`ATTRIBUTE_BONUS_DAMANE=0.3`).

**보조 판정 공식** (각자 0~100% 확률 vs `Random(0,100)`):
- 회피(`:88-131`): auxPoint(속도)차 sqrt + `(회피율 − 명중률 + Random(0,luck))/2`.
- 관통(`:136-193`): `관통율/2 + (ATK−DEF)/5 + Random(0,luck)*명중률/10` + 버프.
- 크리(`:198-263`): `sqrt(aux차/0.09) + 크리율*0.5 + Random(0,luck)` + 버프.

---

## 2. 스탯 — 클라가 빠뜨린 것 (핵심 갭)

`DataDinoCore` (`IWaveCore.cs:615-664`):
- **메인**: `hitPoint`(HP), `attackPoint`, `defencePoint`, **`auxPoint`(순발력=속도)**
- **서브**: `percentHitRate`(명중), `percentPenetRate`(관통), `percentAvoidRate`(회피), `percentCriticalRate`(크리율), `valueCriticalDamage`(크리댐%), `percentResistRate`(CC저항), **`luckPoint`(모든 RNG에 관여)**, `charmPoint`, `currentLevel`, `dinoAttribute`

> ArenaBattle 클라는 **atk/def/hp만** 썼음. **빠진 것 = aux(턴순서+회피+크리), luck(모든 굴림), 명중/관통/회피/크리율, 크리댐, 속성.** 이 갭이 "전투가 밋밋한" 이유.

---

## 3. 나머지 규칙

- **턴 순서** (`WaveStatusActionOrder.cs`): **1순위 auxPoint(속도), 2순위 luck.** 죽은 유닛(HP≤0) 스킵, 라운드마다 순서 재구성.
- **웨이브**: **Squad**(3×3 팀 편성) vs **Raid**(보스 1). 다음 웨이브 체이닝 `Wave._NextWave`.
- **다이노 역할** (Attack/Defence/Speed): **하드코딩 아님, 데이터 기반**. 역할별 계수(`attackPointCoefficient`/`defencePointCoefficient`/`auxPointCoefficient`, `TableData.cs:60-64`)로 스탯이 갈릴 뿐. Attack=공격계수↑, Defence=방어↑, Speed=aux↑.
- **승패** (`WaveStatusWaveFinish.cs:18-28`): 한 진영 `MortalNum <= 0`(전멸)이면 상대 승. 팀 전멸.
- **속성(가위바위보)** (`InteractionNormalFight.cs:34-83`): `NOON(1) < NIGHT(2) < DAWN(3) < NOON` 순환, `ECLIPSE(4)`=중립. 승자 서브스탯 부스트(회피×0.3/크리×0.2/관통×0.1) + **공격자 속성승 시 최종 데미지 +30%**.
- **FSM**: WaveStart→SettingMortals→FirstWave→**ActionOrder**→ProcessSide0/1/PVP→ShowCC→…→WaveFinish.

**상수** (`TableData.cs` `Stat`): `DAMAGE_VALUE=50`, 방어완화 `A/B/C/D=3/91/2/84`, 속성보너스 `회피/크리/관통/데미지=0.3/0.2/0.1/0.3`, 성장 `HP=1000, ATK/DEF/AUX=10000`.

**알려진 버그/미완**(이식 시 주의): 관통 분기가 데미지를 ×0.05~0.1로 **줄임**(TODO 주석); 버프 INCREASE/DECREASE 둘 다 `+= value`(감소가 안 됨, `:168-171`).

---

## 4. 전략: 오프라인 클라에 어떻게 반영할까

세 갈래 (님이 "대개편" 방향에 따라 선택):

**A. 공식만 이식 (가장 작음)** — ArenaBattle의 `ApplyBattleDamage`(임시 `atk*3−def`)를 **`ATK*(50/(DEF+50))` 곡선 + ±분산**으로 교체. 스탯은 일단 atk/def/hp만이라도 진짜 곡선이 되니 밸런스가 살아남. 크리/회피/속성은 점진 추가.
→ **당장 CLI 루프로 검증 가능** (내가 로그 보며 반복).

**B. 스탯셋 + 서브판정 확장** — `DataDinoCore`의 aux/luck/각종 rate를 클라 스탯에 추가하고 회피·크리·속성 판정까지 이식. 진짜 서버 전투에 근접. 중간 규모.

**C. ServerWaveCore를 헤드리스 하네스로 추출 (전략적)** — `ServerWaveCore/`는 순수 C#이라, Unity 의존(UnityEngine/Debug/MonoBehaviour) 걷어내고 MySQL→CSV로 바꿔 **`FruttiDinoRPGConsolServer`에 넣으면 `dotnet run`으로 내가 혼자 돌리는 진짜 전투 시뮬**이 됨. 그럼 룰을 헤드리스로 무한 반복 검증 가능. (아까 얘기한 "순수 C# 시뮬레이터"가 이미 90% 존재)

> 참고: ArenaBattle 클라 전투(`BattleManager` TRIGGER_FSM)와 서버 `ServerWaveCore`(WaveStatus FSM)는 **다른 FSM**이다. A/B는 공식/스탯만 이식(FSM 유지), C는 서버코어 자체를 살림. 관련: `Battle_Rules_Analysis.md`(현행 클라 룰).
