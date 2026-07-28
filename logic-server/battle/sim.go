package battle

import (
	"fmt"
	"math"
	"math/rand"
	"sort"
)

// Battle: 한 판의 전투 상태 + 로그
type Battle struct {
	Dinos     []*Dino
	Log       []string
	Turn      int
	inPassive bool // 패시브가 유발한 타격 처리 중 — 패시브 재귀 발동 방지
}

func (b *Battle) logf(f string, a ...any) { b.Log = append(b.Log, fmt.Sprintf(f, a...)) }

// 턴 순서: 유효 aux(속도) 내림차순, 동률이면 luck.
func (b *Battle) order() []*Dino {
	live := make([]*Dino, 0, len(b.Dinos))
	for _, d := range b.Dinos {
		if d.Alive() {
			live = append(live, d)
		}
	}
	sort.SliceStable(live, func(i, j int) bool {
		ai, aj := live[i].EffAux(), live[j].EffAux()
		if ai != aj {
			return ai > aj
		}
		return live[i].EffLuck() > live[j].EffLuck()
	})
	return live
}

func (b *Battle) sideAlive(side int) int {
	n := 0
	for _, d := range b.Dinos {
		if d.Side == side && d.Alive() {
			n++
		}
	}
	return n
}

// livingSide: 해당 진영의 살아있는 다이노들.
func (b *Battle) livingSide(side int) []*Dino {
	var out []*Dino
	for _, d := range b.Dinos {
		if d.Side == side && d.Alive() {
			out = append(out, d)
		}
	}
	return out
}

func (b *Battle) allies(of *Dino) []*Dino  { return b.livingSide(of.Side) }
func (b *Battle) enemies(of *Dino) []*Dino { return b.livingSide(1 - of.Side) }

// lowestHPRatio: HP 비율이 가장 낮은 대상(회복/버프 우선순위).
func lowestHPRatio(pool []*Dino) *Dino {
	best := pool[0]
	for _, d := range pool[1:] {
		if d.HP/d.MaxHP < best.HP/best.MaxHP {
			best = d
		}
	}
	return best
}

// skillTargets: 스킬 대상 선택. 공격/CC 단일=랜덤 적, 회복/버프 단일=최저 HP 아군.
func (b *Battle) skillTargets(caster *Dino, s *Skill) []*Dino {
	if s.Target == TgtSelf {
		return []*Dino{caster}
	}
	var pool []*Dino
	if s.Target == TgtAlly {
		pool = b.allies(caster)
	} else {
		pool = b.enemies(caster)
	}
	if len(pool) == 0 {
		return nil
	}
	if s.TType == TTAll {
		return pool
	}
	switch s.Action {
	case ActHeal, ActBuff:
		return []*Dino{lowestHPRatio(pool)}
	default:
		return []*Dino{pool[rand.Intn(len(pool))]}
	}
}

// resolveHit: 한 번의 타격 결과를 적용 + 로그. label은 평타/스킬명.
func (b *Battle) resolveHit(a, t *Dino, r AttackResult, label string) {
	if r.Avoided {
		b.logf("[T%d] %s =%s=> %s : 회피!", b.Turn, a.Name, label, t.Name)
		return
	}
	t.HP -= r.Damage
	if t.HP < 0 {
		t.HP = 0
	}
	tag := ""
	if r.Crit {
		tag += " CRIT"
	}
	if r.AttrWin == 0 {
		tag += " 속성+"
	}
	if r.Penetrated {
		tag += " 관통!"
	} else if r.Defended {
		tag += fmt.Sprintf(" 방어(-%.0f%%)", r.DefenceRate)
	}
	b.logf("[T%d] %s =%s=> %s : -%.0f%s (hp %.0f/%.0f)", b.Turn, a.Name, label, t.Name, r.Damage, tag, t.HP, t.MaxHP)
	died := !t.Alive()
	if died {
		b.logf("      * %s 사망 (아군 %d / 적 %d)", t.Name, b.sideAlive(0), b.sideAlive(1))
	}
	// 패시브 트리거: 공격자(공격/처치) → 피격자(피격/사망). 순서 = 공격시 → 처치시 → 사망시/피격시.
	b.fireEvent(a, t, OnAttack, r.Damage)
	if died {
		b.fireEvent(a, t, OnKill, r.Damage)
		b.fireEvent(t, a, OnDeath, r.Damage)
	} else {
		b.fireEvent(t, a, OnHit, r.Damage)
	}
}

// basicAttack: 평타 — 랜덤 적 1명 공격.
func (b *Battle) basicAttack(a *Dino) {
	enemies := b.enemies(a)
	if len(enemies) == 0 {
		return
	}
	t := enemies[rand.Intn(len(enemies))]
	b.resolveHit(a, t, Attack(a, t), "평타")
}

// signedNum: +25 / -25 형태 문자열.
func signedNum(v float64) string {
	if v >= 0 {
		return fmt.Sprintf("+%.0f", v)
	}
	return fmt.Sprintf("%.0f", v)
}

func statName(s StatKind) string {
	switch s {
	case StatAttack:
		return "공격력"
	case StatDefence:
		return "방어력"
	case StatAux:
		return "순발력"
	case StatHitRate:
		return "명중률"
	case StatAvoidRate:
		return "회피율"
	case StatPenetRate:
		return "관통율"
	case StatCritRate:
		return "치명확률"
	case StatLuck:
		return "행운"
	case StatResist:
		return "상태저항"
	}
	return "?"
}

// castSkill: 액티브 스킬 시전. 대상이 없으면 평타로 대체.
func (b *Battle) castSkill(a *Dino, s *Skill) {
	targets := b.skillTargets(a, s)
	if len(targets) == 0 {
		b.basicAttack(a)
		return
	}
	b.applyAction(a, s, targets, s.Name)
}

// applyAction: 스킬 액션 1건을 대상들에 적용 + 로그. 액티브·패시브 공용.
// label = 표시 이름(액티브=스킬명, 패시브=패시브명).
func (b *Battle) applyAction(a *Dino, s *Skill, targets []*Dino, label string) {
	switch s.Action {
	case ActAttack:
		for _, t := range targets {
			r := Attack(a, t)
			r.Damage *= s.Power
			b.resolveHit(a, t, r, label)
		}
	case ActHeal:
		for _, t := range targets {
			before := t.HP
			t.HP += s.Power
			if t.HP > t.MaxHP {
				t.HP = t.MaxHP
			}
			b.logf("[T%d] %s =%s=> %s : +%.0f 회복 (hp %.0f/%.0f)", b.Turn, a.Name, label, t.Name, t.HP-before, t.HP, t.MaxHP)
		}
	case ActBuff, ActDebuff:
		sign, word := 1.0, "버프"
		if s.Action == ActDebuff {
			sign, word = -1.0, "디버프"
		}
		unit := ""
		if s.Op == OpPercent {
			unit = "%"
		}
		delta := sign * math.Abs(s.Delta)
		for _, t := range targets {
			if s.Action == ActDebuff && isResisted(a, t) { // 디버프 저항
				b.logf("[T%d] %s =%s=> %s : 저항! (%s 무효)", b.Turn, a.Name, label, t.Name, statName(s.Stat))
				continue
			}
			t.Effects = append(t.Effects, &Effect{
				Kind: EffBuff, Name: label, Stat: s.Stat, Op: s.Op, Delta: delta, Remain: s.Dur,
			})
			b.logf("[T%d] %s =%s=> %s : <%s> %s %s%s (%d턴)", b.Turn, a.Name, label, t.Name, word, statName(s.Stat), signedNum(delta), unit, s.Dur)
		}
	case ActCC:
		for _, t := range targets {
			if isResisted(a, t) { // CC 저항
				b.logf("[T%d] %s =%s=> %s : 저항! (%s 무효)", b.Turn, a.Name, label, t.Name, s.CC.Name)
				continue
			}
			t.Effects = append(t.Effects, &Effect{
				Kind: EffCC, Name: s.CC.Name, ActLock: s.CC.ActLock, DoT: s.CC.DoT, Remain: s.CC.Duration,
			})
			kind := "행동불가"
			if s.CC.DoT > 0 {
				kind = fmt.Sprintf("지속피해 %.0f", s.CC.DoT)
			}
			b.logf("[T%d] %s =%s=> %s : [%s] %s (%d턴)", b.Turn, a.Name, label, t.Name, s.CC.Name, kind, s.CC.Duration)
		}
	case ActCleanse:
		for _, t := range targets {
			if n := t.removeDebuffs(); n > 0 {
				b.logf("[T%d] %s =%s=> %s : 디버프/CC %d개 해제", b.Turn, a.Name, label, t.Name, n)
			}
		}
	}
}

// startTurn: 턴 시작 처리 — 지속피해 적용 → 효과 지속턴 감소 → 쿨다운 감소.
// 반환: (행동불가 여부, CC 이름). 지속피해로 죽었으면 Alive()가 false가 된다.
func (b *Battle) startTurn(a *Dino) (bool, string) {
	if dot := a.dotTotal(); dot > 0 {
		a.HP -= dot
		if a.HP < 0 {
			a.HP = 0
		}
		b.logf("[T%d] %s 지속피해 -%.0f (hp %.0f/%.0f)", b.Turn, a.Name, dot, a.HP, a.MaxHP)
		if !a.Alive() {
			b.logf("      * %s 사망 (아군 %d / 적 %d)", a.Name, b.sideAlive(0), b.sideAlive(1))
			b.fireEvent(a, nil, OnDeath, dot) // 지속피해로 사망 시에도 사망 트리거
		}
	}
	locked, ccName := a.ccLocked()
	a.decayEffects()
	if a.Active != nil {
		a.Active.cool()
	}
	for _, p := range a.Passives {
		p.cool()
	}
	return locked, ccName
}

// Run: 완전 자동 전투. 반환 winner side (0 아군 / 1 적 / -1 무승부·시간초과)
func (b *Battle) Run(maxTurns int) int {
	for round := 1; ; round++ {
		b.logf("--- Round %d ---", round)
		for _, a := range b.order() {
			if !a.Alive() {
				continue
			}
			if b.sideAlive(0) == 0 || b.sideAlive(1) == 0 {
				goto done
			}
			b.Turn++
			if b.Turn > maxTurns {
				b.logf("[MAX] 턴 초과(%d)", maxTurns)
				goto done
			}

			locked, ccName := b.startTurn(a)
			if !a.Alive() { // 지속피해로 사망
				continue
			}
			if locked {
				b.logf("[T%d] %s — [%s]로 행동 불가", b.Turn, a.Name, ccName)
				continue
			}

			if a.Active != nil && a.Active.Ready() {
				b.castSkill(a, a.Active)
				a.Active.fire()
			} else {
				b.basicAttack(a)
			}
		}
	}
done:
	if b.sideAlive(1) == 0 {
		return 0
	}
	if b.sideAlive(0) == 0 {
		return 1
	}
	return -1
}
