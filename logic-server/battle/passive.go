package battle

import "math/rand"

// [스펙 출처] ServerWaveCore의 트리거 모델(Trigger_* 다수). 원본은 CSV 골격 + Debug.Log
// 스텁이라 실행 로직이 없어, 최소 실동작(이벤트 반응형 패시브)만 여기서 구현한다.

// TriggerEvent: 패시브가 반응하는 전투 이벤트. [스펙] TriggerTBL을 최대한 이식.
type TriggerEvent int

const (
	OnAttack     TriggerEvent = iota // 공격 명중(공격자) — Target Hited/Defpen
	OnHit                            // 피해 입고 생존(피격자) — Me Hited
	OnKill                           // 적 처치(공격자) — Me Kill / Target dead
	OnDeath                          // 사망(사망자) — Me Dead
	OnCrit                           // 크리 명중(공격자) — Me Crihit / MainAct Crihit
	OnAvoid                          // 회피 성공(회피자) — Me Avd
	OnCCed                           // 상태이상에 걸림(피대상) — Me Cc
	OnLowHP                          // HP가 임계(5%) 미만으로 하락 — MyHp Less
	OnAllyKill                       // 아군이 적을 처치(생존 아군) — Ally Kill
	OnAllyDeath                      // 아군이 사망(생존 아군) — Ally Dead
	OnStageStart                     // 스테이지 시작 — Stage Start
	OnWaveStart                      // 웨이브 시작 — Wave Start
	OnTurnStart                      // 자기 턴 시작 — Turn Start
	OnTurnEnd                        // 자기 턴 종료 — Turn end

	lowHPThreshold = 0.05 // OnLowHP 발동 HP 비율
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
	case OnCrit:
		return "치명시"
	case OnAvoid:
		return "회피시"
	case OnCCed:
		return "피상태이상시"
	case OnLowHP:
		return "위기시"
	case OnAllyKill:
		return "아군처치시"
	case OnAllyDeath:
		return "아군사망시"
	case OnStageStart:
		return "스테이지시작"
	case OnWaveStart:
		return "웨이브시작"
	case OnTurnStart:
		return "턴시작"
	case OnTurnEnd:
		return "턴종료"
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

// fireAll: 살아있는 모든 다이노에 타이밍 이벤트 발동(스테이지/웨이브 시작 등).
func (b *Battle) fireAll(ev TriggerEvent) {
	for _, d := range b.Dinos {
		b.fireEvent(d, nil, ev, 0)
	}
}

// fireAllies: of의 살아있는 아군(of 제외)에 이벤트 발동. other=사건 당사자.
func (b *Battle) fireAllies(of *Dino, ev TriggerEvent, other *Dino) {
	for _, d := range b.Dinos {
		if d.Side == of.Side && d != of {
			b.fireEvent(d, other, ev, 0)
		}
	}
}
