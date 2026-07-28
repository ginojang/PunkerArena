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

// 회피 성공? aux(속도)차 sqrt + (회피율 - 명중률 + rand(0,luck))/2. 유효(버프반영) 스탯 사용.
func isAvoid(a, d *Dino) bool {
	da, aa := d.EffAux(), a.EffAux()
	var rate float64
	if da > aa {
		rate += math.Sqrt(da - aa)
	}
	rate += (d.EffAvoidRate() - a.EffHitRate() + rand.Float64()*d.EffLuck()) / 2.0
	return rand.Float64()*100.0 < rate
}

// 크리 성공? sqrt(aux차/0.09) + 크리율*0.5 + rand(0,luck). 유효(버프반영) 스탯 사용.
func isCrit(a, d *Dino) bool {
	auxDiff := a.EffAux() - d.EffAux()
	var rate float64
	if auxDiff > 0 {
		rate += math.Sqrt(auxDiff / 0.09)
	}
	rate += a.EffCritRate()*0.5 + rand.Float64()*a.EffLuck()
	return rand.Float64()*100.0 < rate
}

// 방어(Defence)모드 대상 피해 경감율(%). [이식] GetDefenceDamageRate :356-401.
//   ATK>=DEF : -A*sqrt(ATK-DEF)+B   (A,B=3,91)
//   ATK< DEF :  C*sqrt(DEF-ATK)+D   (C,D=2,84)
// → 84~91% 경감. [0,100] 클램프.
// 원본 버그: NormalAttackTo가 이 값을 /100 없이 `dmg -= dmg*rate` 로 곱해 데미지가
// 음수가 됐음. 여기선 의도대로 %(=/100)로 적용한다.
func defenceDamageRate(atk, def float64) float64 {
	var r float64
	if atk >= def {
		r = -DefA*math.Sqrt(atk-def) + DefB
	} else {
		r = DefC*math.Sqrt(def-atk) + DefD
	}
	if r > 100 {
		r = 100
	}
	if r < 0 {
		r = 0
	}
	return r
}

// 방어관통 성공? [이식] isPenetrateSuccess :136-193.
// 관통율/2 + (ATK-DEF)/5 + rand(0,luck)*명중률/10. [0,100] 클램프 후 rand(0,100) 비교.
func isPenetrate(a, d *Dino) bool {
	rate := a.EffPenetRate() / 2.0
	rate += (a.EffAttack() - d.EffDefence()) / 5.0
	rate += rand.Float64() * a.EffLuck() * a.EffHitRate() / 10.0
	if rate > 100 {
		rate = 100
	}
	if rate < 0 {
		rate = 0
	}
	return rand.Float64()*100.0 < rate
}

// isResisted: 대상 d가 공격자 a의 CC/디버프를 저항하는가.
// [신규 규칙] 원본은 저항 스탯만 있고 판정이 스텁 → 여기서 정의.
//   저항율(유효) + (대상 행운 - 공격자 행운)/2, [0,100] 클램프 후 rand(0,100) 비교.
//   (행운 동률이면 저항율이 곧 성공확률.)
func isResisted(a, d *Dino) bool {
	rate := d.EffResist() + (d.EffLuck()-a.EffLuck())/2.0
	if rate < 0 {
		rate = 0
	}
	if rate > 100 {
		rate = 100
	}
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
	Penetrated  bool    // 방어모드 대상을 관통했는가
	Defended    bool    // 방어모드 경감이 적용됐는가
	DefenceRate float64 // 적용된 경감율(%) — Defended일 때만
	AttrWin     int     // 0=공격자 속성승, 1=방어자, -1=없음
}

// 공격 판정: 회피 -> 크리 -> 속성보너스 -> (대상 방어모드면) 관통/경감. [이식] NormalAttackTo.
func Attack(a, d *Dino) AttackResult {
	if isAvoid(a, d) {
		return AttackResult{Avoided: true, AttrWin: -1}
	}
	atk, def := a.EffAttack(), d.EffDefence()
	base := baseDamage(atk, def)
	dmg := base
	crit := isCrit(a, d)
	if crit {
		dmg = critDamage(base, def, a.Level, d.Level, a.CritDamage)
	}
	aw := attributeWinner(a.Attribute, d.Attribute)
	if aw == 0 {
		dmg += dmg * AttributeBonusDamage
	}

	res := AttackResult{Crit: crit, AttrWin: aw}
	if d.Defending {
		if isPenetrate(a, d) {
			// 관통: 방어 경감을 무시하고 풀 데미지.
			// 원본 버그: 관통 성공 시 오히려 *0.05/*0.1로 감소시킴(TODO 주석) → 여기선 경감 무시로 수정.
			res.Penetrated = true
			res.Damage = dmg
			return res
		}
		rate := defenceDamageRate(atk, def)
		dmg -= dmg * (rate / 100.0)
		res.Defended = true
		res.DefenceRate = rate
	}
	res.Damage = dmg
	return res
}
