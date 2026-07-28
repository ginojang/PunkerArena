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

// BuildDino: idx의 다이노를 level까지 성장시켜 생성. rng로 재현 가능(같은 시드=같은 결과).
// [이식] CreateDino(1레벨 결정) + ProcessLevelUp(레벨업 성장). 파츠/룬 미포함(body 계수만),
// 부스탯(명중/치명/저항 등)은 파츠 전용이라 여기선 기본값을 준다.
func (t *Tables) BuildDino(idx, level int, rng *rand.Rand, name string, side int) (*battle.Dino, error) {
	b, ok := t.Bases[idx]
	if !ok {
		return nil, fmt.Errorf("DinoBaseTBL에 idx %d 없음", idx)
	}
	if level < 1 {
		level = 1
	}

	// ── 1레벨 결정 ────────────────────────────────────────────────
	// 계수 ±2 랜덤 후 성장용 base로 저장(초기치 곱/10분배 이전 값).
	baseHp := b.HpCoef + jitter(rng)
	baseAtk := b.AtkCoef + jitter(rng)
	baseDef := b.DefCoef + jitter(rng)
	baseAux := b.SpdCoef + jitter(rng)

	// +10 랜덤 분배 후 초기치계수 곱: 스탯_base = init_coef * coef / 100.
	dh, da, dd, dx := distribute10(rng)
	hp := b.InitCoef * (baseHp + dh) / 100.0
	atk := b.InitCoef * (baseAtk + da) / 100.0
	def := b.InitCoef * (baseDef + dd) / 100.0
	aux := b.InitCoef * (baseAux + dx) / 100.0

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

	d := &battle.Dino{
		Name: name, Side: side,
		HP: HP, MaxHP: HP, Attack: ATK, Defence: DEF, Aux: AUX,
		// 부스탯: 파츠 미모델 → 합리적 기본값(추후 DinoPartsTBL 반영 예정).
		HitRate: 50, AvoidRate: 5, PenetRate: 20, CritRate: 10, CritDamage: 50, Luck: 10, Resist: 10,
		Level: level, Attribute: battle.Attribute(b.Attribute),
	}
	return d, nil
}
