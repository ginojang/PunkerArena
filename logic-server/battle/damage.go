package battle

import (
	"math"
	"math/rand"
)

// [이식] InteractionNormalFight.cs 의 데미지/판정 공식.
// 기본 데미지: ATK*(50/(DEF+50)), ±1/16 분산. ATK<DEF 면 칩데미지 1.
func baseDamage(atk, def float64) float64 {
	if atk < def {
		return 1
	}
	b := atk * (DamageValue / (def + DamageValue))
	lo, hi := b*15.0/16.0, b*17.0/16.0
	return lo + rand.Float64()*(hi-lo)
}

// 크리 데미지: base + DEF*(atkLv/defLv)*0.5 + base*(critDmg%/100)
func critDamage(base, def float64, atkLv, defLv int, critDmgPct float64) float64 {
	if defLv <= 0 {
		defLv = 1
	}
	c := base + def*(float64(atkLv)/float64(defLv))*0.5
	c += base * (critDmgPct / 100.0)
	return c
}

// 회피 성공? aux(속도)차 sqrt + (회피율 - 명중률 + rand(0,luck))/2
func isAvoid(a, d *Dino) bool {
	var rate float64
	if d.Aux > a.Aux {
		rate += math.Sqrt(d.Aux - a.Aux)
	}
	rate += (d.AvoidRate - a.HitRate + rand.Float64()*d.Luck) / 2.0
	return rand.Float64()*100.0 < rate
}

// 크리 성공? sqrt(aux차/0.09) + 크리율*0.5 + rand(0,luck)
func isCrit(a, d *Dino) bool {
	auxDiff := a.Aux - d.Aux
	var rate float64
	if auxDiff > 0 {
		rate += math.Sqrt(auxDiff / 0.09)
	}
	rate += a.CritRate*0.5 + rand.Float64()*a.Luck
	return rand.Float64()*100.0 < rate
}

// 속성 승자: 0=공격자 승, 1=방어자 승, -1=없음. NOON<NIGHT<DAWN<NOON(순환), ECLIPSE 중립.
func attributeWinner(atk, def Attribute) int {
	if atk == Eclipse || def == Eclipse || atk == AttrNone || def == AttrNone {
		return -1
	}
	d := ((int(atk) - int(def)) + 3) % 3
	switch d {
	case 1:
		return 0
	case 2:
		return 1
	}
	return -1
}

// AttackResult: 한 번의 공격 결과
type AttackResult struct {
	Damage      float64
	Crit        bool
	Avoided     bool
	AttrWin     int // 0=공격자 속성승, 1=방어자, -1=없음
}

// 공격 판정: 회피 -> 크리 -> 속성보너스 순 (defence모드/관통은 후속 확장)
func Attack(a, d *Dino) AttackResult {
	if isAvoid(a, d) {
		return AttackResult{Avoided: true, AttrWin: -1}
	}
	base := baseDamage(a.Attack, d.Defence)
	dmg := base
	crit := isCrit(a, d)
	if crit {
		dmg = critDamage(base, d.Defence, a.Level, d.Level, a.CritDamage)
	}
	aw := attributeWinner(a.Attribute, d.Attribute)
	if aw == 0 {
		dmg += dmg * AttributeBonusDamage
	}
	return AttackResult{Damage: dmg, Crit: crit, AttrWin: aw}
}
