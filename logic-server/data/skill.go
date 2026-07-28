package data

import (
	"fmt"
	"path/filepath"
	"strings"

	"punker/logic-server/battle"
)

// [스펙 출처] SkillTBL(구조) + SkillLevelTBL(레벨별 수치) + SkillBuffTBL/SkillCcTBL(효과 정의).
// 원본 enum: SkillType(ACTIVE=1,PASSIVE=2), SkillTarget(SELF1/ALLY2/ENEMY3),
// SkillTargetType(SINGLE1/ALL2/…), SkillAction(ATTACK1/RECOVERY2/BUFF_DEBUF4/CC5/…).
// 원본 실행부는 스텁이었으므로, 데이터 의미를 여기서 battle 모델로 매핑한다.

const (
	skActive  = 1
	skPassive = 2

	actAttack  = 1
	actRecover = 2
	actBuffDbf = 4
	actCC      = 5

	statCleanse = 14 // SkillBuffTBL stat_type 14 = 상태이상 해제(res_clear)
)

// SubActDef: SkillTBL의 sub_act1/sub_act2 (main과 동일 구조의 2차 효과).
type SubActDef struct {
	Index              int // 0=sub_act1, 1=sub_act2 (레벨 수치 인덱스)
	Trigger, Action    int
	Id                 int
	Target, TargetType int
}

// SkillDef: SkillTBL 한 행(전투 컬럼만).
type SkillDef struct {
	Idx, Type, Cool      int
	Target, TargetType   int
	MainTrigger, MainAct int
	MainActId            int
	Name                 string      // StringTBL에서 해석한 실제 이름
	Subs                 []SubActDef // 채워진 sub_act (action!=0)만
}

// SkillLevel: SkillLevelTBL 한 (idx,lv) 행의 main_act + sub_act 수치.
type SkillLevel struct {
	Lv        int
	Rate      float64 // 발동 확률(%)
	Ref       int     // 참조 스탯(1/2=atk 계열 — 근사)
	ValueType int     // 2=퍼센트
	Value     float64
	Turn      int

	SubRate  [2]float64 // sub_act1/2 발동 확률
	SubValue [2]float64
	SubTurn  [2]int
}

// BuffDef: SkillBuffTBL — 버프/디버프 정의.
type BuffDef struct {
	Idx, BuffType, Calc, StatType int // BuffType 1버프/2디버프, Calc ±2%/±1상수/0특수
}

// CcDef: SkillCcTBL — 상태이상 정의.
type CcDef struct {
	Idx, ActLock int    // ActLock 1=행동불가
	Name         string // StringTBL에서 해석한 실제 이름(기절/중독 등)
}

func headerIndex(header []string) map[string]int {
	m := map[string]int{}
	for i, h := range header {
		m[strings.TrimSpace(strings.TrimPrefix(h, "\ufeff"))] = i
	}
	return m
}

func cell(row []string, i int) string {
	if i >= 0 && i < len(row) {
		return row[i]
	}
	return ""
}

// loadSkills: 4개 스킬 테이블을 Tables에 로드.
func (t *Tables) loadSkills(dir string) error {
	// SkillTBL
	rows, err := readCSV(filepath.Join(dir, "SkillTBL.csv"))
	if err != nil {
		return fmt.Errorf("SkillTBL: %w", err)
	}
	t.Skills = map[int]SkillDef{}
	h := headerIndex(rows[0])
	for _, r := range rows[1:] {
		if strings.TrimSpace(cell(r, h["idx"])) == "" {
			continue
		}
		sd := SkillDef{
			Idx: atoi(cell(r, h["idx"])), Type: atoi(cell(r, h["type"])), Cool: atoi(cell(r, h["cool_turn"])),
			Target: atoi(cell(r, h["target"])), TargetType: atoi(cell(r, h["target_type"])),
			MainTrigger: atoi(cell(r, h["main_trigger"])), MainAct: atoi(cell(r, h["main_act"])),
			MainActId: atoi(cell(r, h["main_act_id"])),
			Name:      t.str(atoi(cell(r, h["name"]))), // name 컬럼 = StringTBL id
		}
		for i, pre := range []string{"sub_act1", "sub_act2"} {
			act := atoi(cell(r, h[pre])) // sub_act1 / sub_act2 = 액션 컬럼
			if act == 0 {
				continue
			}
			sd.Subs = append(sd.Subs, SubActDef{
				Index: i, Trigger: atoi(cell(r, h[pre+"_trigger"])), Action: act,
				Id: atoi(cell(r, h[pre+"_id"])), Target: atoi(cell(r, h[pre+"_target"])),
				TargetType: atoi(cell(r, h[pre+"_target_type"])),
			})
		}
		t.Skills[sd.Idx] = sd
	}

	// SkillLevelTBL
	rows, err = readCSV(filepath.Join(dir, "SkillLevelTBL.csv"))
	if err != nil {
		return fmt.Errorf("SkillLevelTBL: %w", err)
	}
	t.SkillLevels = map[int]map[int]SkillLevel{}
	h = headerIndex(rows[0])
	for _, r := range rows[1:] {
		idx := atoi(cell(r, h["idx"]))
		if idx == 0 && strings.TrimSpace(cell(r, h["idx"])) == "" {
			continue
		}
		sl := SkillLevel{
			Lv: atoi(cell(r, h["lv"])), Rate: atof(cell(r, h["main_act_rate"])), Ref: atoi(cell(r, h["main_act_ref"])),
			ValueType: atoi(cell(r, h["main_act_value_type"])), Value: atof(cell(r, h["main_act_value"])),
			Turn: atoi(cell(r, h["main_act_turn"])),
		}
		// sub_act 수치(고유 컬럼명). sub2의 ref/value_type은 테이블 오타로 중복명이라 미사용.
		sl.SubRate[0], sl.SubValue[0], sl.SubTurn[0] = atof(cell(r, h["sub_act1_rate"])), atof(cell(r, h["sub_act1_value"])), atoi(cell(r, h["sub_act1_turn"]))
		sl.SubRate[1], sl.SubValue[1], sl.SubTurn[1] = atof(cell(r, h["sub_act2_rate"])), atof(cell(r, h["sub_act2_value"])), atoi(cell(r, h["sub_act2_turn"]))
		if t.SkillLevels[idx] == nil {
			t.SkillLevels[idx] = map[int]SkillLevel{}
		}
		t.SkillLevels[idx][sl.Lv] = sl
	}

	// SkillBuffTBL
	rows, err = readCSV(filepath.Join(dir, "SkillBuffTBL.csv"))
	if err != nil {
		return fmt.Errorf("SkillBuffTBL: %w", err)
	}
	t.Buffs = map[int]BuffDef{}
	h = headerIndex(rows[0])
	for _, r := range rows[1:] {
		if strings.TrimSpace(cell(r, h["idx"])) == "" || strings.TrimSpace(cell(r, h["stat_type"])) == "" {
			continue
		}
		bd := BuffDef{Idx: atoi(cell(r, h["idx"])), BuffType: atoi(cell(r, h["buff_type"])),
			Calc: atoi(cell(r, h["buff_calculation"])), StatType: atoi(cell(r, h["stat_type"]))}
		t.Buffs[bd.Idx] = bd
	}

	// SkillCcTBL
	rows, err = readCSV(filepath.Join(dir, "SkillCcTBL.csv"))
	if err != nil {
		return fmt.Errorf("SkillCcTBL: %w", err)
	}
	t.Ccs = map[int]CcDef{}
	h = headerIndex(rows[0])
	for _, r := range rows[1:] {
		if strings.TrimSpace(cell(r, h["idx"])) == "" {
			continue
		}
		cd := CcDef{Idx: atoi(cell(r, h["idx"])), ActLock: atoi(cell(r, h["type_act_lock"])),
			Name: t.str(atoi(cell(r, h["name"])))} // name 컬럼 = StringTBL id
		t.Ccs[cd.Idx] = cd
	}
	return nil
}

// pickLevel: 요청 레벨 이하의 최고 레벨 행, 없으면 최저 행.
func (t *Tables) pickLevel(idx, want int) (SkillLevel, bool) {
	m := t.SkillLevels[idx]
	if len(m) == 0 {
		return SkillLevel{}, false
	}
	best, bestLv, found := SkillLevel{}, -1, false
	minLv, min := 1 <<30, SkillLevel{}
	for lv, sl := range m {
		if lv < minLv {
			minLv, min = lv, sl
		}
		if lv <= want && lv > bestLv {
			bestLv, best, found = lv, sl, true
		}
	}
	if found {
		return best, true
	}
	return min, true
}

// statTypeToKind: SkillBuffTBL stat_type → battle.StatKind. (0=미지원)
func statTypeToKind(st int) (battle.StatKind, bool) {
	switch st {
	case 2:
		return battle.StatAttack, true
	case 3:
		return battle.StatDefence, true
	case 4:
		return battle.StatAux, true
	case 5:
		return battle.StatHitRate, true
	case 6:
		return battle.StatPenetRate, true
	case 7:
		return battle.StatAvoidRate, true
	case 8:
		return battle.StatCritRate, true
	case 10:
		return battle.StatResist, true
	case 11:
		return battle.StatLuck, true
	}
	return 0, false
}

// triggerEvent: TriggerTBL idx → battle.TriggerEvent. 미모델링 트리거는 (0,false).
// [미지원] 11 Me Alive(상시아우라), 13 Target Cc, 16~20·32 Burstcombo, 21 예약스킬,
//          29 Shield, 31 Target Defence, 33·34 Def/Atk 임계 아우라 — 전투모델 밖.
func triggerEvent(trig int) (battle.TriggerEvent, bool) {
	switch trig {
	case 1: // Stage Start
		return battle.OnStageStart, true
	case 2: // Wave Start
		return battle.OnWaveStart, true
	case 3: // Turn Start
		return battle.OnTurnStart, true
	case 4, 23: // Me Kill Enemy / Target dead
		return battle.OnKill, true
	case 5, 7: // Me Hited
		return battle.OnHit, true
	case 6: // Me Cc
		return battle.OnCCed, true
	case 8, 24, 25, 26, 27: // Me Crihit / MainAct Crihit
		return battle.OnCrit, true
	case 9: // Me Avd
		return battle.OnAvoid, true
	case 10: // Me Dead
		return battle.OnDeath, true
	case 12, 28: // Target Hited(내 공격 명중) / Defpen
		return battle.OnAttack, true
	case 14: // Ally Kill Enemy
		return battle.OnAllyKill, true
	case 15: // Ally Dead
		return battle.OnAllyDeath, true
	case 22: // Turn end
		return battle.OnTurnEnd, true
	case 30: // MyHp Less
		return battle.OnLowHP, true
	}
	return 0, false
}

func mapTarget(tg int) battle.Target {
	switch tg {
	case 1:
		return battle.TgtSelf
	case 2:
		return battle.TgtAlly
	}
	return battle.TgtEnemy
}

func mapTType(tt int) battle.TargetType {
	if tt == 2 { // ALL
		return battle.TTAll
	}
	return battle.TTSingle // SINGLE 및 ROW/COL/FRONT/FUTHER는 단일로 근사
}

// mapPTarget: 패시브 대상(이벤트 문맥) — 적 대상=상대, 자신=자신, 아군=아군전체.
func mapPTarget(tg int) battle.PassiveTarget {
	switch tg {
	case 1:
		return battle.PSelf
	case 2:
		return battle.PAllies
	}
	return battle.POther
}

func actionKor(act int) string {
	switch act {
	case actAttack:
		return "공격"
	case actRecover:
		return "회복"
	case actBuffDbf:
		return "버프"
	case actCC:
		return "상태이상"
	}
	return "스킬"
}

// buildAction: 스킬의 메인 액션 페이로드. caster는 스탯 스케일용.
func (t *Tables) buildAction(sd SkillDef, sl SkillLevel, caster *battle.Dino) (*battle.Skill, error) {
	name := sd.Name
	if name == "" { // StringTBL 미해석 시 합성 이름 폴백
		name = fmt.Sprintf("%s#%d", actionKor(sd.MainAct), sd.Idx)
	}
	return t.buildActionRaw(sd.MainAct, sd.MainActId, name, sl.Value, sl.Turn, caster)
}

// buildActionRaw: (액션·참조id·이름·수치·지속)로 액션 페이로드 생성. main/sub 공용.
func (t *Tables) buildActionRaw(action, actId int, name string, value float64, turn int, caster *battle.Dino) (*battle.Skill, error) {
	s := &battle.Skill{Name: name}
	switch action {
	case actAttack:
		s.Action = battle.ActAttack
		s.Power = value / 100.0 // value=150 → 1.5배
		if s.Power <= 0 {
			s.Power = 1.0
		}
	case actRecover:
		s.Action = battle.ActHeal
		s.Power = caster.Attack * value / 100.0 // 시전자 공격력의 value% 회복(근사)
	case actBuffDbf:
		bd, ok := t.Buffs[actId]
		if !ok {
			return nil, fmt.Errorf("SkillBuffTBL %d 없음", actId)
		}
		if bd.StatType == statCleanse { // res_clear → 클렌즈
			s.Action = battle.ActCleanse
			return s, nil
		}
		kind, ok := statTypeToKind(bd.StatType)
		if !ok {
			return nil, fmt.Errorf("버프 stat_type %d 미지원", bd.StatType)
		}
		s.Stat = kind
		if bd.Calc == 1 || bd.Calc == -1 {
			s.Op = battle.OpConst
		} else {
			s.Op = battle.OpPercent
		}
		s.Delta = value
		s.Dur = turn
		if bd.BuffType == 2 {
			s.Action = battle.ActDebuff
		} else {
			s.Action = battle.ActBuff
		}
	case actCC:
		cd, ok := t.Ccs[actId]
		if !ok {
			return nil, fmt.Errorf("SkillCcTBL %d 없음", actId)
		}
		s.Action = battle.ActCC
		dur := turn
		if dur <= 0 {
			dur = 1
		}
		ccName := cd.Name
		if ccName == "" {
			ccName = fmt.Sprintf("CC%d", cd.Idx)
		}
		if cd.ActLock == 1 { // 기절/빙결류
			s.CC = battle.CCSpec{Name: ccName, ActLock: true, Duration: dur}
		} else { // 비행동제약 + value → 지속피해(중독/출혈)로 근사
			dot := caster.Attack * value / 100.0
			if dot <= 0 {
				dot = value
			}
			s.CC = battle.CCSpec{Name: ccName, DoT: dot, Duration: dur}
		}
	default:
		return nil, fmt.Errorf("액션 %d 미지원", action)
	}
	return s, nil
}

// attachSubs: sd의 sub_act들을 처리. trig=0=메인과 함께 발동(라이더), trig!=0=이벤트 패시브.
// 미지원 액션/트리거인 sub는 조용히 스킵.
func (t *Tables) attachSubs(d *battle.Dino, sd SkillDef, sl SkillLevel, main *battle.Skill) {
	for _, sub := range sd.Subs {
		subName := fmt.Sprintf("%s·%s", main.Name, actionKor(sub.Action))
		sp, err := t.buildActionRaw(sub.Action, sub.Id, subName, sl.SubValue[sub.Index], sl.SubTurn[sub.Index], d)
		if err != nil {
			continue // 미지원 서브액션(STEALTH/CRITICAL 등)
		}
		sp.Target = mapTarget(sub.Target)
		sp.TType = mapTType(sub.TargetType)
		if sub.Trigger == 0 { // 메인과 함께 발동
			main.Riders = append(main.Riders, sp)
			continue
		}
		ev, ok := triggerEvent(sub.Trigger)
		if !ok {
			continue // 미모델링 트리거
		}
		chance := sl.SubRate[sub.Index]
		if chance >= 100 {
			chance = 0
		}
		d.Passives = append(d.Passives, &battle.Passive{
			Name: fmt.Sprintf("P:%s", subName), Event: ev, PTarget: mapPTarget(sub.Target),
			Skill: sp, Chance: chance, MaxCool: sd.Cool,
		})
	}
}

// BuildSkillOn: skillIdx 스킬을 dino에 장착(액티브=Active, 패시브=Passives 추가).
// caster 스탯 스케일 반영. 미지원 액션/트리거면 error(호출부에서 폴백).
func (t *Tables) BuildSkillOn(d *battle.Dino, skillIdx, skillLv int) error {
	sd, ok := t.Skills[skillIdx]
	if !ok {
		return fmt.Errorf("SkillTBL idx %d 없음", skillIdx)
	}
	sl, ok := t.pickLevel(skillIdx, skillLv)
	if !ok {
		return fmt.Errorf("SkillLevelTBL idx %d 레벨행 없음", skillIdx)
	}
	payload, err := t.buildAction(sd, sl, d)
	if err != nil {
		return err
	}
	t.attachSubs(d, sd, sl, payload) // 2차 효과(라이더/서브 패시브) 부착

	if sd.Type == skPassive || sd.MainTrigger != 0 { // 패시브(트리거 有)
		ev, ok := triggerEvent(sd.MainTrigger)
		if !ok {
			return fmt.Errorf("트리거 %d 미지원", sd.MainTrigger)
		}
		chance := sl.Rate
		if chance >= 100 {
			chance = 0 // 100%는 무조건 발동(Chance 0 처리)
		}
		d.Passives = append(d.Passives, &battle.Passive{
			Name: fmt.Sprintf("P:%s", payload.Name), Event: ev, PTarget: mapPTarget(sd.Target),
			Skill: payload, Chance: chance, MaxCool: sd.Cool,
		})
		return nil
	}

	// 액티브
	payload.Target = mapTarget(sd.Target)
	payload.TType = mapTType(sd.TargetType)
	payload.MaxCool = sd.Cool
	d.Active = payload
	return nil
}

// DinoSkillIDs: 다이노가 보유하는 스킬 = 몸통 대표 스킬 + 장착 파츠들의 스킬(중복/0 제외).
// [모델] 원본: 몸통 skill_slot + 파츠 skill 컬럼이 다이노 스킬셋을 구성.
func (t *Tables) DinoSkillIDs(dinoIdx int) []int {
	b, ok := t.Bases[dinoIdx]
	if !ok {
		return nil
	}
	var out []int
	seen := map[int]bool{}
	add := func(id int) {
		if id > 0 && !seen[id] {
			seen[id] = true
			out = append(out, id)
		}
	}
	add(b.Skill)
	for _, p := range t.bestParts(b.Class) {
		add(p.Skill)
	}
	return out
}

// DescribeSkills: 다이노가 파생한 스킬 풀을 사람이 읽을 문자열로. (예: "1010101(공격),1020404(버프)")
func (t *Tables) DescribeSkills(dinoIdx int) string {
	ids := t.DinoSkillIDs(dinoIdx)
	if len(ids) == 0 {
		return "-"
	}
	parts := make([]string, 0, len(ids))
	for _, id := range ids {
		nm, tag := fmt.Sprintf("#%d", id), "?"
		if sd, ok := t.Skills[id]; ok {
			if sd.Name != "" {
				nm = sd.Name
			}
			tag = actionKor(sd.MainAct)
			if sd.Type == skPassive || sd.MainTrigger != 0 {
				tag += "/패시브"
			}
		}
		parts = append(parts, fmt.Sprintf("%s(%s)", nm, tag))
	}
	return strings.Join(parts, ", ")
}

// AutoEquipSkills: 몸통+파츠에서 파생된 스킬들을 다이노에 장착.
// 액티브는 (내 모델상 슬롯 1개라) 가장 높은 idx 하나만 주 스킬로, 패시브는 모두 부착한다.
// 미지원 스킬은 조용히 건너뛴다(로그 노이즈 방지).
func (t *Tables) AutoEquipSkills(d *battle.Dino, dinoIdx, skillLv int) {
	primaryActive := 0
	for _, id := range t.DinoSkillIDs(dinoIdx) {
		sd, ok := t.Skills[id]
		if !ok {
			continue
		}
		if sd.Type != skPassive && sd.MainTrigger == 0 { // 액티브 후보 → 최고 idx 하나만
			if id > primaryActive {
				primaryActive = id
			}
			continue
		}
		_ = t.BuildSkillOn(d, id, skillLv) // 패시브: 지원되면 부착
	}
	if primaryActive != 0 {
		_ = t.BuildSkillOn(d, primaryActive, skillLv)
	}
}
