package data

import (
	"fmt"
	"math/rand"

	"punker/logic-server/battle"
)

// [이식] 성장 상수 (ServerWaveCore TableData.Stat).
const (
	hitPointGrowth = 1000.0
	otherGrowth    = 10000.0 // ATK/DEF/AUX 공통
	bonusRandom    = 10      // 레벨당 랜덤 분배 포인트
)

// jitter: 계수에 ±2 랜덤. [이식] SystemTool.Random(-2, 2+1).
func jitter(rng *rand.Rand) float64 { return float64(rng.Intn(5) - 2) }

// distribute10: bonusRandom(10) 포인트를 4스탯에 랜덤 분배. [이식] BONUS_RANDOM_POINT 루프.
func distribute10(rng *rand.Rand) (hp, atk, def, aux float64) {
	for i := 0; i < bonusRandom; i++ {
		switch rng.Intn(4) {
		case 0:
			hp++
		case 1:
			atk++
		case 2:
			def++
		case 3:
			aux++
		}
	}
	return
}

// bestParts: 클래스의 파츠 슬롯(parts_type 1..5)마다 부스탯 총합이 가장 큰 파츠 1개 선택.
// [모델] 원본은 가챠로 파츠를 조합(고정표 없음) → 헤드리스에선 "슬롯별 최적 파츠 장착"으로 근사.
func (t *Tables) bestParts(class int) []PartRow {
	best := map[int]PartRow{} // type → 최적 파츠
	for _, p := range t.Parts {
		if p.Class != class || p.Type < 1 || p.Type > 5 {
			continue
		}
		cur, ok := best[p.Type]
		if !ok || p.subSum() > cur.subSum() || (p.subSum() == cur.subSum() && p.Idx < cur.Idx) {
			best[p.Type] = p
		}
	}
	out := make([]PartRow, 0, len(best))
	for slot := 1; slot <= 5; slot++ {
		if p, ok := best[slot]; ok {
			out = append(out, p)
		}
	}
	return out
}

// BuildDino: idx의 다이노를 level까지 성장시켜 생성. rng로 재현 가능(같은 시드=같은 결과).
// [이식] CreateDino(1레벨 결정) + ProcessLevelUp(레벨업 성장). 몸통 계수 + 장착 파츠 계수 합산,
// 부스탯(명중/치명/저항 등)은 파츠에서 산출.
func (t *Tables) BuildDino(idx, level int, rng *rand.Rand, name string, side int) (*battle.Dino, error) {
	b, ok := t.Bases[idx]
	if !ok {
		return nil, fmt.Errorf("DinoBaseTBL에 idx %d 없음", idx)
	}
	if level < 1 {
		level = 1
	}

	// 장착 파츠: 몸통 계수에 파츠 계수를 더하고, 부스탯은 파츠 합으로 산출한다.
	parts := t.bestParts(b.Class)
	initCoef, hpCoef, atkCoef, defCoef, spdCoef := b.InitCoef, b.HpCoef, b.AtkCoef, b.DefCoef, b.SpdCoef
	for _, p := range parts {
		initCoef += p.InitCoef
		hpCoef += p.HpCoef
		atkCoef += p.AtkCoef
		defCoef += p.DefCoef
		spdCoef += p.SpdCoef
	}

	// ── 1레벨 결정 ────────────────────────────────────────────────
	// 계수 ±2 랜덤 후 성장용 base로 저장(초기치 곱/10분배 이전 값).
	baseHp := hpCoef + jitter(rng)
	baseAtk := atkCoef + jitter(rng)
	baseDef := defCoef + jitter(rng)
	baseAux := spdCoef + jitter(rng)

	// +10 랜덤 분배 후 초기치계수 곱: 스탯_base = init_coef * coef / 100.
	dh, da, dd, dx := distribute10(rng)
	hp := initCoef * (baseHp + dh) / 100.0
	atk := initCoef * (baseAtk + da) / 100.0
	def := initCoef * (baseDef + dd) / 100.0
	aux := initCoef * (baseAux + dx) / 100.0

	// 최종 1레벨 스탯(스탯 간 교차 가중).
	HP := hp*4.0 + atk + def + aux
	ATK := hp*0.1 + atk + def*0.1 + aux*0.05
	DEF := hp*0.1 + atk*0.1 + def + aux*0.05
	AUX := aux

	// ── 레벨업 성장(2..level) ─────────────────────────────────────
	rr := t.rankOf(baseHp + baseAtk + baseDef + baseAux) // GrowthBase = base 4스탯 합
	for lv := 2; lv <= level; lv++ {
		constValue := float64(rng.Intn(int(rr.Max-rr.Min)+1)) + rr.Min
		hr, ar, dr, xr := distribute10(rng)
		HP += (baseHp + hr) * (constValue / hitPointGrowth)
		ATK += (baseAtk + ar) * (constValue / otherGrowth)
		DEF += (baseDef + dr) * (constValue / otherGrowth)
		AUX += (baseAux + xr) * (constValue / otherGrowth)
	}

	// 부스탯: 장착 파츠에서 합산(원본 CreateDino: 파츠 부스탯 누적). 파츠가 없으면 0.
	var hit, avd, cri, criDmg, res, pen, luk float64
	for _, p := range parts {
		hit += p.HitRate
		avd += p.AvoidRate
		cri += p.CritRate
		criDmg += p.CritDmg
		res += p.ResRate
		pen += p.PenRate
		luk += p.Luck
	}

	d := &battle.Dino{
		Name: name, Side: side,
		HP: HP, MaxHP: HP, Attack: ATK, Defence: DEF, Aux: AUX,
		HitRate: hit, AvoidRate: avd, PenetRate: pen, CritRate: cri, CritDamage: criDmg, Luck: luk, Resist: res,
		Level: level, Attribute: battle.Attribute(b.Attribute),
	}
	return d, nil
}
