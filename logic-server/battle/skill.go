package battle

// [스펙 출처] ServerWaveCore의 SkillCore(target/targetType/action) + XSkillInfo(maxCoolTurn).
// 원본은 로더/골격만 있었고 실행은 미구현 → 여기서 실제 액티브 스킬을 동작시킨다.

// Target: 스킬 대상 진영.
type Target int

const (
	TgtSelf  Target = iota // 자신
	TgtAlly                // 아군
	TgtEnemy               // 적군
)

// TargetType: 대상 범위.
type TargetType int

const (
	TTSingle TargetType = iota // 단일
	TTAll                      // 전체
)

// Action: 스킬이 하는 일.
type Action int

const (
	ActAttack Action = iota // 공격(평타 대비 배수 데미지)
	ActHeal                 // 회복
	ActBuff                 // 아군 스탯 증가
	ActDebuff               // 적 스탯 감소
	ActCC                   // 상태이상 부여
)

// CCSpec: 상태이상 정의. ActLock=행동불가(기절/빙결/수면), DoT>0=지속피해(출혈/중독).
type CCSpec struct {
	Name     string
	ActLock  bool
	DoT      float64
	Duration int
}

// Skill: 액티브 스킬 1개. 쿨다운이 차면 평타 대신 사용한다.
type Skill struct {
	Name    string
	Target  Target
	TType   TargetType
	Action  Action

	Power float64 // ActAttack: 평타 대비 배수 / ActHeal: 회복량

	Stat  StatKind // ActBuff/ActDebuff 대상 스탯
	Op    Operator // ActBuff/ActDebuff 연산
	Delta float64  // ActBuff/ActDebuff 크기(부호는 Action이 결정)
	Dur   int      // ActBuff/ActDebuff 지속턴

	CC CCSpec // ActCC 상태이상

	MaxCool int // 최대 쿨타임(턴)
	cur     int // 현재 쿨다운(0 이하면 사용 가능)
}

// Ready: 지금 사용 가능한가.
func (s *Skill) Ready() bool { return s.cur <= 0 }

// fire: 사용 후 쿨다운을 최대치로 리셋.
func (s *Skill) fire() { s.cur = s.MaxCool }

// cool: 쿨다운 1 감소.
func (s *Skill) cool() {
	if s.cur > 0 {
		s.cur--
	}
}

// reset: 쿨다운 초기화(웨이브 전환 시).
func (s *Skill) reset() { s.cur = 0 }
