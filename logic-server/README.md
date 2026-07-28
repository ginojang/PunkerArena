# logic-server (Go)

PunkerArena 전투 로직 서버 — **Go로 신설**. 언어 결정: C# 재사용을 버리고 Go 채택(단일바이너리·헤드리스 반복검증 우선). 관련 결정 배경: 루트 `Server_Battle_Core_Reference.md`, `Battle_Rules_Analysis.md`.

## 목표
- 서버권위 턴제 웨이브 전투를 **순수 Go로** 구현 (규칙은 `FruttiDino_RPG/Server/.../ServerWaveCore` 이식).
- **헤드리스로 `go run`** 해서 룰을 반복검증 (Unity 없이).
- 추후 WebSocket 계층(WebGL 호환) 추가 → Unity 클라와 연결. 매칭/소셜 필요 시 Nakama를 앞단에.

## 구조 (현재: 첫 마일스톤)
```
logic-server/
  battle/          전투 코어 (순수 Go, 테스트/헤드리스 가능)
    stat.go        Dino 스탯셋(DataDinoCore 이식) + 상수(TableData.Stat)
    damage.go      진짜 데미지 공식: ATK*(50/(DEF+50)) ±1/16, 크리, 회피, 속성 RPS(+30%)
    sim.go         단일 전투 루프: 속도(aux)순 턴 → 턴시작 틱(DoT·CC·감쇠·쿨) → 스킬/평타 → 전멸 승패
    wave.go        스테이지=편대(HP 이월) vs 순차 적 웨이브 체이닝
    effect.go      지속효과: 버프/디버프(스탯 레이어) + CC(행동불가/지속피해), Remain 턴 감쇠
    skill.go       액티브 스킬 정의: 대상(자신/아군/적)·범위(단일/전체)·액션(공격/힐/버프/디버프/CC)·쿨다운
    passive.go     패시브 트리거: 14종 이벤트(공격/피격/처치/사망/치명/회피/피CC/위기/아군처치/아군사망/스테이지·웨이브·턴시작/턴종료), 확률·쿨, 재귀 가드
  data/            밸런스 테이블 로더 (Table/csv vendored 사본 → battle.Dino/Skill)
    loader.go      CSV 파싱(BOM 제거) + 성장 랭크 선택
    build.go       실제 스탯/성장 공식: 1레벨 결정 + 레벨업 성장(rank×constValue) + 파츠 계수·부스탯
    skill.go       SkillTBL/SkillLevelTBL/SkillBuffTBL/SkillCcTBL → battle.Skill/Passive 매핑 (main_act + sub_act1/2)
    csv/           Dino*TBL(스탯/성장/파츠) + Skill*TBL/TriggerTBL(스킬) + StringTBL(이름)
  cmd/sim/main.go  헤드리스 스테이지 러너 (편대·적을 CSV 실스탯+레벨+실스킬로 편성)
```

## 실행
```
cd logic-server
go run ./cmd/sim      # 3v3 오토배틀 로그 + 승패 출력
```

## 이식 현황 (ServerWaveCore 대비)
- [x] 기본 데미지 곡선 `ATK*(50/(DEF+50))` ±분산, `ATK<DEF→1`
- [x] 크리 데미지, 크리/회피 판정(aux·luck 기반)
- [x] 속성 가위바위보(NOON<NIGHT<DAWN 순환, ECLIPSE 중립, 승리 +30%)
- [x] 속도(aux)순 턴 순서 + 전멸 승패
- [x] 방어(Defence)모드 완화 84~91% + 관통(경감 무시). 원본 버그 2개(경감 /100 누락, 관통 ×0.05/0.1 감소) 의도대로 수정
- [x] 웨이브 체이닝: 편대(side 0) HP 이월·사망 유지 vs 순차 신규 적 웨이브. 전 웨이브 클리어=승, 편대 전멸=패
- [x] 액티브 스킬: 대상(자신/아군/적)·범위(단일/전체)·쿨다운. 액션 5종 — 배율공격/회복/버프/디버프/CC
- [x] 지속효과: 버프·디버프(스탯 % 또는 정수 레이어, 유효스탯 반영) + CC(기절=행동불가, 중독/출혈=지속피해), 턴당 감쇠·웨이브 전환 리셋
- [x] 패시브 트리거: 이벤트 발동(공격시/피격시/처치시/사망시) → 액티브와 동일 액션(공격/힐/버프/디버프/CC) 재사용, 확률·쿨다운, 대상(자신/상대/적전체/아군전체). 패시브가 유발한 타격은 재귀 발동 안 함(무한루프 방지)
- [x] CC·디버프 저항(Resist 스탯 굴림: `저항율 + (행운차)/2`, CC·디버프 부여 시 판정) + 클렌즈(ActCleanse: 대상 아군의 CC·음수버프 제거, 이로운 버프는 유지)
- [x] 실데이터 로딩 + 레벨 성장: `data/` 가 Table/csv(DinoBaseTBL 계수) → 실제 스탯 생성. 1레벨 결정(`init_coef×coef/100` 후 교차가중) + 레벨업 성장(GrowthBase→rank→`constValue/GROWTH`). 편대·적을 idx+레벨로 편성
- [x] 파츠(DinoPartsTBL): 슬롯(parts_type 1~5)별 최적 파츠 자동 장착 → 파츠 메인계수를 몸통에 합산 + 부스탯(명중/회피/치명/치명뎀/저항/관통/행운)을 파츠 합으로 산출(원본 CreateDino 방식). grade-1 파츠라 부스탯은 실제로 작음(크리 희소)
- [x] 파츠→스킬 자동 파생: 다이노 스킬셋 = 몸통 skill + 장착 파츠들의 skill 컬럼. `AutoEquipSkills`가 액티브(내 모델상 1슬롯 → 최고 idx 하나) + 지원되는 패시브 모두 장착. 로스터에 `파생풀[…] → 장착[…]` 출력. 클래스별로 공격/버프/CC/힐이 자연히 갈림(예: class2→버프, class4→CC 보유)
- [x] StringTBL 실제 이름: SkillTBL.name·SkillCcTBL.name = StringTBL id → 실제 스킬/CC명 해석(초고속 꼬리, 랜덤 야자 미사일, 기절, 중독 등). 버프는 stat 기반 표기 유지(SkillBuffTBL.name은 placeholder). 미해석 시 `액션#idx` 폴백
- [x] sub_act 로딩: SkillTBL의 sub_act1/2(2차 효과, main과 동일 구조)를 매핑. trig=0 → 스킬 사용 시 함께 발동(Riders, 각자 대상), trig!=0 → 이벤트 서브 패시브. 미지원 액션(STEALTH 등)·트리거는 스킵. 예: 공격 스킬에 자기 순발력 버프/적 기절 라이더가 붙음(로스터 `+2차 N`)
- [x] 트리거 완전 매핑: TriggerTBL 34종 전부 처리 — 18종→이벤트(스테이지/웨이브/턴시작·턴종료, 공격/치명/피격/회피/피CC/위기/처치/사망/아군처치/아군사망), 16종(버스트콤보/실드/aura임계 등 전투모델 밖)은 명시적 미지원. 실데이터 main 트리거(1/2/3/12/28)는 31(Target Defence) 빼고 전부 발동. `turn=0` 버프는 상시(영구) 아우라로 처리(스테이지 내내 유지). 이전에 드롭되던 스테이지시작 패시브(치명/방어/공격 버프)가 이제 발동
- [x] 스킬 CSV 로딩: SkillTBL(구조)+SkillLevelTBL(레벨수치)+SkillBuffTBL/SkillCcTBL(효과) → `BuildSkillOn`이 액티브/패시브로 매핑. 액션 ATTACK(배율)/RECOVERY(atk% 회복)/BUFF_DEBUF(스탯·%·해제)/CC(행동불가·DoT), 트리거(Kill/Hited/Dead/Defpen→OnKill/OnHit/OnDeath/OnAttack) 근사. 미지원 액션·트리거는 평타 폴백. 스킬명은 StringTBL 미로드로 `액션#idx` 합성
- [ ] WebSocket 서버 + 클라 프로토콜

상수/공식 출처는 `Server_Battle_Core_Reference.md` 참고.
