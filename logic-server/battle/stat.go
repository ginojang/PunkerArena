package battle

// [이식] 서버권위 전투 상수 (ServerWaveCore TableData.cs 'Stat' 기준)
const (
	DamageValue          = 50.0 // 방어완화 곡선: ATK*(DamageValue/(DEF+DamageValue))
	AttributeBonusDamage = 0.30 // 속성 승리 시 최종 데미지 +30%

	// 방어(Defence)모드 완화율: ATK>=DEF -> -A*sqrt(ATK-DEF)+B ; else C*sqrt(DEF-ATK)+D
	DefA, DefB, DefC, DefD = 3.0, 91.0, 2.0, 84.0
)

// 속성(가위바위보): NOON<NIGHT<DAWN<NOON(순환), ECLIPSE=중립
type Attribute int

const (
	AttrNone Attribute = 0
	Noon     Attribute = 1
	Night    Attribute = 2
	Dawn     Attribute = 3
	Eclipse  Attribute = 4
)

// DataDinoCore 이식 — 클라(atk/def/hp만)가 빠뜨린 스탯 전부 포함
type Dino struct {
	Name string
	Side int // 0 = 아군, 1 = 적

	// 메인 스탯
	HP, MaxHP        float64
	Attack, Defence  float64
	Aux              float64 // 순발력(속도): 턴순서 + 회피 + 크리에 관여

	// 서브 스탯
	HitRate    float64 // 명중률
	AvoidRate  float64 // 회피율
	CritRate   float64 // 크리율
	CritDamage float64 // 크리 데미지(%)
	Luck       float64 // 모든 RNG 굴림에 관여
	Level      int
	Attribute  Attribute
}

func (d *Dino) Alive() bool { return d.HP > 0 }
