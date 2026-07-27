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

	// 플레이어 편대(side 0) — 웨이브 간 HP 이월, 사망 유지
	allyDef := mkDino("Ally_Def", 0, 200, 28, 55, 15, battle.Night)
	allyDef.Defending = true // 방어모드: 피해 84~91% 경감 (관통에만 뚫림)
	squad := []*battle.Dino{
		mkDino("Ally_Atk", 0, 120, 40, 20, 30, battle.Noon),
		allyDef,
		mkDino("Ally_Spd", 0, 100, 30, 15, 50, battle.Dawn),
	}

	// 적 웨이브들(side 1) — 웨이브마다 신규 그룹, 뒤로 갈수록 강해짐
	stage := &battle.Stage{
		Squad: squad,
		Waves: [][]*battle.Dino{
			{
				mkDino("W1_Grunt_A", 1, 80, 28, 15, 20, battle.Night),
				mkDino("W1_Grunt_B", 1, 80, 26, 18, 25, battle.Dawn),
			},
			{
				mkDino("W2_Raider_A", 1, 100, 34, 22, 30, battle.Noon),
				mkDino("W2_Raider_B", 1, 100, 30, 25, 22, battle.Night),
				mkDino("W2_Raider_C", 1, 90, 32, 18, 35, battle.Dawn),
			},
			{
				mkDino("W3_Boss", 1, 220, 44, 26, 28, battle.Eclipse),
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
