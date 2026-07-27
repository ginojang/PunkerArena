package main

import (
	"fmt"
	"math/rand"

	"punker/logic-server/battle"
)

// 헤드리스 전투 시뮬레이터: go run ./cmd/sim
// 3v3 오토배틀을 돌리고 로그 + 승패를 출력한다. (룰 반복검증용)
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

	allyDef := mkDino("Ally_Def", 0, 160, 20, 60, 15, battle.Night)
	allyDef.Defending = true // 방어모드: 피해 84~91% 경감 (관통에만 뚫림)
	enemyB := mkDino("Enemy_B", 1, 120, 25, 30, 20, battle.Dawn)
	enemyB.Defending = true

	b := &battle.Battle{}
	b.Dinos = append(b.Dinos,
		mkDino("Ally_Atk", 0, 100, 40, 20, 30, battle.Noon),
		allyDef,
		mkDino("Ally_Spd", 0, 90, 30, 15, 50, battle.Dawn),
		mkDino("Enemy_A", 1, 100, 30, 20, 25, battle.Night),
		enemyB,
		mkDino("Enemy_C", 1, 80, 35, 10, 40, battle.Noon),
	)

	w := b.Run(500)
	for _, l := range b.Log {
		fmt.Println(l)
	}
	res := "무승부/시간초과"
	if w == 0 {
		res = "WIN (아군 승리)"
	} else if w == 1 {
		res = "LOSE (적 승리)"
	}
	fmt.Printf("\n=== 전투 종료: %s (총 %d턴) ===\n", res, b.Turn)
}
