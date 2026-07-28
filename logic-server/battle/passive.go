package battle

import "math/rand"

// [스펙 출처] ServerWaveCore의 트리거 모델(Trigger_* 다수). 원본은 CSV 골격 + Debug.Log
// 스텁이라 실행 로직이 없어, 최소 실동작(이벤트 반응형 패시브)만 여기서 구현한다.

// TriggerEvent: 패시브가 반응하는 전투 이벤트.
type TriggerEvent int

const (
	OnAttack TriggerEvent = iota // 이 다이노의 공격이 명중했을 때(공격자 기준)
	OnHit                        // 이 다이노가 피해를 입고 생존했을 때(피격자 기준)
	OnKill                       // 이 다이노가 적을 처치했을 때(공격자 기준)
	OnDeath                      // 이 다이노가 사망했을 때(사망자 기준)
)

// PassiveTarget: 패시브 액션의 대상(이벤트 문맥 기준).
type PassiveTarget int

const (
	PSelf    PassiveTarget = iota // 패시브 소유자
	POther                        // 이벤트 상대(피격 시 공격자 / 처치 시 피해자)
	PEnemies                      // 소유자의 살아있는 적 전체
	PAllies                       // 소유자의 살아있는 아군 전체
)

// Passive: 조건 발동형 스킬. 이벤트 발생 시 Skill 액션을 대상에 적용한다.
// Skill의 Action/Power/Stat/Op/Delta/Dur/CC를 재사용하고, Skill.Target/TType은 무시한다
// (대상은 PTarget이 이벤트 문맥으로 결정).
type Passive struct {
	Name    string
	Event   TriggerEvent
	PTarget PassiveTarget
	Skill   *Skill  // 발동 시 적용할 액션 페이로드
	Chance  float64 // 발동 확률(%) 0 이하 = 항상
	MaxCool int     // 발동 후 재발동까지 쿨(턴). 0 = 매번
	cur     int
}

func (p *Passive) ready() bool { return p.cur <= 0 }
func (p *Passive) fire()       { p.cur = p.MaxCool }
func (p *Passive) cool() {
	if p.cur > 0 {
		p.cur--
	}
}
func (p *Passive) reset() { p.cur = 0 }

func eventName(ev TriggerEvent) string {
	switch ev {
	case OnAttack:
		return "공격시"
	case OnHit:
		return "피격시"
	case OnKill:
		return "처치시"
	case OnDeath:
		return "사망시"
	}
	return "?"
}

// passiveTargets: 이벤트 문맥으로 패시브 대상 선택.
func (b *Battle) passiveTargets(owner, other *Dino, p *Passive) []*Dino {
	switch p.PTarget {
	case PSelf:
		return []*Dino{owner}
	case POther:
		if other == nil || !other.Alive() {
			return nil
		}
		return []*Dino{other}
	case PEnemies:
		return b.enemies(owner)
	case PAllies:
		return b.allies(owner)
	}
	return nil
}

// fireEvent: owner의 패시브 중 ev에 반응하는 것을 발동. other = 이벤트 상대(없으면 nil).
// 패시브가 유발한 타격에서는 재귀 발동을 막는다(inPassive 가드) — 무한루프 방지.
func (b *Battle) fireEvent(owner, other *Dino, ev TriggerEvent, dmg float64) {
	if b.inPassive || len(owner.Passives) == 0 {
		return
	}
	// 생존 조건: 사망시 트리거는 죽었을 때만, 그 외는 살아있을 때만.
	if ev == OnDeath {
		if owner.Alive() {
			return
		}
	} else if !owner.Alive() {
		return
	}
	for _, p := range owner.Passives {
		if p.Event != ev || !p.ready() {
			continue
		}
		if p.Chance > 0 && rand.Float64()*100.0 >= p.Chance {
			continue
		}
		targets := b.passiveTargets(owner, other, p)
		if len(targets) == 0 {
			continue
		}
		b.logf("[T%d] %s <패시브:%s> 발동 (%s)", b.Turn, owner.Name, p.Name, eventName(ev))
		b.inPassive = true
		b.applyAction(owner, p.Skill, targets, p.Name)
		b.inPassive = false
		p.fire()
	}
}
