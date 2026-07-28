package battle

// [스펙 출처] ServerWaveCore의 BonusStat.Kind / SkillCC / SkillBuff 데이터 모델.
// 원본은 이 모델만 정의하고 실행은 스텁이었으므로, 실제 동작을 여기서 구현한다.

// StatKind: 버프/디버프가 대상으로 삼는 스탯.
type StatKind int

const (
	StatAttack StatKind = iota
	StatDefence
	StatAux
	StatHitRate
	StatAvoidRate
	StatPenetRate
	StatCritRate
	StatLuck
)

// Operator: 버프 연산 방식.
type Operator int

const (
	OpConst   Operator = iota // 정수 가감
	OpPercent                 // 기준 스탯의 % 가감
)

// EffectKind: 지속 효과 분류.
type EffectKind int

const (
	EffBuff EffectKind = iota // 스탯 증감(버프/디버프)
	EffCC                     // 상태이상(행동불가 / 지속피해)
)

// Effect: 다이노에 걸린 지속 효과 1개. Remain 턴이 지나면 만료.
type Effect struct {
	Kind    EffectKind
	Name    string
	Stat    StatKind // EffBuff 대상 스탯
	Op      Operator // EffBuff 연산
	Delta   float64  // EffBuff 증감량(부호 포함: 디버프=음수)
	DoT     float64  // EffCC 턴당 지속피해(출혈/중독)
	ActLock bool     // EffCC 행동 불가(기절/빙결/수면)
	Remain  int
}

// statOf: 기준값 base에 활성 버프/디버프를 모두 적용한 유효 스탯.
func (d *Dino) statOf(base float64, s StatKind) float64 {
	v := base
	for _, e := range d.Effects {
		if e.Kind != EffBuff || e.Stat != s {
			continue
		}
		switch e.Op {
		case OpConst:
			v += e.Delta
		case OpPercent:
			v += base * e.Delta / 100.0
		}
	}
	if v < 0 {
		v = 0
	}
	return v
}

func (d *Dino) EffAttack() float64    { return d.statOf(d.Attack, StatAttack) }
func (d *Dino) EffDefence() float64   { return d.statOf(d.Defence, StatDefence) }
func (d *Dino) EffAux() float64       { return d.statOf(d.Aux, StatAux) }
func (d *Dino) EffHitRate() float64   { return d.statOf(d.HitRate, StatHitRate) }
func (d *Dino) EffAvoidRate() float64 { return d.statOf(d.AvoidRate, StatAvoidRate) }
func (d *Dino) EffPenetRate() float64 { return d.statOf(d.PenetRate, StatPenetRate) }
func (d *Dino) EffCritRate() float64  { return d.statOf(d.CritRate, StatCritRate) }
func (d *Dino) EffLuck() float64      { return d.statOf(d.Luck, StatLuck) }

// ccLocked: 현재 행동 불가 CC가 걸려있는가.
func (d *Dino) ccLocked() (bool, string) {
	for _, e := range d.Effects {
		if e.Kind == EffCC && e.ActLock && e.Remain > 0 {
			return true, e.Name
		}
	}
	return false, ""
}

// dotTotal: 이번 턴 받을 지속피해 총량.
func (d *Dino) dotTotal() float64 {
	var t float64
	for _, e := range d.Effects {
		t += e.DoT
	}
	return t
}

// decayEffects: 모든 효과의 지속턴을 1 감소시키고 만료된 것을 제거.
func (d *Dino) decayEffects() {
	out := d.Effects[:0]
	for _, e := range d.Effects {
		e.Remain--
		if e.Remain > 0 {
			out = append(out, e)
		}
	}
	d.Effects = out
}

// clearEffects: 모든 지속 효과 제거(웨이브 전환 시 리셋용).
func (d *Dino) clearEffects() { d.Effects = nil }
