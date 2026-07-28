package main

import (
	"fmt"
	"math/rand"

	"punker/logic-server/battle"
)

// 헤드리스 전투 시뮬레이터: go run ./cmd/sim
// 플레이어 편대가 순차 적 웨이브를 상대하는 스테이지를 돌리고 로그 + 승패를 출력한다. (룰 반복검증용)
func mkDino(name string, side int, hp, atk, def, aux float64, attr battle.Attribute) *battle.Dino {
	return &battle.Dino{
		Name: name, Side: side,
		HP: hp, MaxHP: hp, Attack: atk, Defence: def, Aux: aux,
		HitRate: 50, AvoidRate: 5, PenetRate: 20, CritRate: 10, CritDamage: 50, Luck: 10,
		Level: 1, Attribute: attr,
	}
}

func main() {
	rand.Seed(1) // 재현 가능 (deprecated지만 결정적 시드용)

	// ── 플레이어 편대(side 0): HP 웨이브 이월 + 각자 액티브 스킬 ─────────────
	allyAtk := mkDino("Ally_Atk", 0, 120, 40, 20, 30, battle.Noon)
	allyAtk.Active = &battle.Skill{ // 강타: 적 단일에 2.2배 데미지
		Name: "강타", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActAttack, Power: 2.2, MaxCool: 3,
	}
	allyAtk.Passives = []*battle.Passive{{ // 광폭: 적 처치 시 자신 공격력 +20% 3턴(스노볼)
		Name: "광폭", Event: battle.OnKill, PTarget: battle.PSelf,
		Skill: &battle.Skill{Action: battle.ActBuff, Stat: battle.StatAttack, Op: battle.OpPercent, Delta: 20, Dur: 3},
	}}

	allyDef := mkDino("Ally_Def", 0, 200, 28, 55, 15, battle.Night)
	allyDef.Defending = true         // 방어모드: 피해 84~91% 경감(관통에만 뚫림)
	allyDef.Active = &battle.Skill{ // 방어호령: 아군 전체 방어력 +25% 3턴
		Name: "방어호령", Target: battle.TgtAlly, TType: battle.TTAll,
		Action: battle.ActBuff, Stat: battle.StatDefence, Op: battle.OpPercent, Delta: 25, Dur: 3, MaxCool: 4,
	}
	allyDef.Passives = []*battle.Passive{{ // 가시갑옷: 피격 시 60% 확률로 공격자에게 반격(평타 0.7배)
		Name: "가시갑옷", Event: battle.OnHit, PTarget: battle.POther, Chance: 60,
		Skill: &battle.Skill{Action: battle.ActAttack, Power: 0.7},
	}}

	allySpd := mkDino("Ally_Spd", 0, 100, 30, 15, 50, battle.Dawn)
	allySpd.Active = &battle.Skill{ // 치유: 최저 HP 아군 +45 회복
		Name: "치유", Target: battle.TgtAlly, TType: battle.TTSingle,
		Action: battle.ActHeal, Power: 45, MaxCool: 3,
	}

	squad := []*battle.Dino{allyAtk, allyDef, allySpd}

	// ── 적 웨이브(side 1): 웨이브마다 신규 그룹, 일부는 스킬 보유 ──────────────
	w1b := mkDino("W1_Grunt_B", 1, 80, 26, 18, 60, battle.Dawn) // 편대보다 빨라 죽기 전에 맹독을 건다
	w1b.Active = &battle.Skill{ // 맹독: 적 단일에 중독(턴당 7, 3턴)
		Name: "맹독", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActCC, CC: battle.CCSpec{Name: "중독", DoT: 7, Duration: 3}, MaxCool: 2,
	}

	w2a := mkDino("W2_Raider_A", 1, 100, 34, 22, 30, battle.Noon)
	w2a.Active = &battle.Skill{ // 약화: 적 단일 공격력 -25% 2턴
		Name: "약화", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActDebuff, Stat: battle.StatAttack, Op: battle.OpPercent, Delta: 25, Dur: 2, MaxCool: 3,
	}

	w3boss := mkDino("W3_Boss", 1, 220, 44, 26, 28, battle.Eclipse)
	w3boss.Active = &battle.Skill{ // 공포: 적 단일 1턴 기절
		Name: "공포", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActCC, CC: battle.CCSpec{Name: "기절", ActLock: true, Duration: 1}, MaxCool: 3,
	}
	w3boss.Passives = []*battle.Passive{{ // 최후의 발악: 사망 시 편대 전원에게 1.2배 광역 피해
		Name: "최후의발악", Event: battle.OnDeath, PTarget: battle.PEnemies,
		Skill: &battle.Skill{Action: battle.ActAttack, Power: 1.2},
	}}

	stage := &battle.Stage{
		Squad: squad,
		Waves: [][]*battle.Dino{
			{
				mkDino("W1_Grunt_A", 1, 80, 28, 15, 20, battle.Night),
				w1b,
			},
			{
				w2a,
				mkDino("W2_Raider_B", 1, 100, 30, 25, 22, battle.Night),
				mkDino("W2_Raider_C", 1, 90, 32, 18, 35, battle.Dawn),
			},
			{
				w3boss,
				mkDino("W3_Guard", 1, 120, 26, 30, 18, battle.Night),
			},
		},
	}

	res := stage.Run(500)
	for _, l := range stage.Log {
		fmt.Println(l)
	}
	verdict := "LOSE (편대 전멸)"
	if res.Win {
		verdict = "WIN (스테이지 클리어)"
	}
	fmt.Printf("\n=== 스테이지 종료: %s — 웨이브 %d/%d 클리어, 총 %d턴 ===\n",
		verdict, res.WavesClear, res.TotalWaves, res.Turns)
}
