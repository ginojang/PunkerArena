package main

import (
	"fmt"
	"math/rand"

	"punker/logic-server/battle"
	"punker/logic-server/data"
)

// 헤드리스 전투 시뮬레이터: go run ./cmd/sim
// 편대/적을 실제 밸런스 테이블(Table/csv vendored 사본)에서 레벨 성장까지 계산해 생성하고,
// 순차 웨이브 스테이지를 돌려 로그 + 승패를 출력한다. (룰 반복검증용)

const csvDir = "data/csv" // `go run ./cmd/sim` 기준 상대경로

func attrName(a battle.Attribute) string {
	switch a {
	case battle.Noon:
		return "정오"
	case battle.Night:
		return "밤"
	case battle.Dawn:
		return "새벽"
	case battle.Eclipse:
		return "월식"
	}
	return "-"
}

// mustBuild: 테이블에서 다이노 생성(실패 시 패닉 — 데모용).
func mustBuild(t *data.Tables, idx, level int, rng *rand.Rand, name string, side int) *battle.Dino {
	d, err := t.BuildDino(idx, level, rng, name, side)
	if err != nil {
		panic(err)
	}
	return d
}

func main() {
	rand.Seed(1)                          // 전투 판정 RNG(재현용)
	buildRng := rand.New(rand.NewSource(42)) // 다이노 생성 RNG(전투와 분리)

	tables, err := data.LoadTables(csvDir)
	if err != nil {
		fmt.Println("테이블 로드 실패:", err)
		fmt.Println("→ logic-server/ 디렉터리에서 `go run ./cmd/sim` 로 실행하세요.")
		return
	}

	// ── 플레이어 편대(side 0): 실제 다이노를 레벨 성장시켜 편성 + 스킬 부여 ──────
	allyAtk := mustBuild(tables, 111, 60, buildRng, "Ally_Atk", 0) // 정오, 딜러형
	allyAtk.Active = &battle.Skill{ // 강타
		Name: "강타", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActAttack, Power: 2.2, MaxCool: 3,
	}
	allyAtk.Passives = []*battle.Passive{
		{Name: "광폭", Event: battle.OnKill, PTarget: battle.PSelf,
			Skill: &battle.Skill{Action: battle.ActBuff, Stat: battle.StatAttack, Op: battle.OpPercent, Delta: 20, Dur: 3}},
		{Name: "재생", Event: battle.OnKill, PTarget: battle.PAllies,
			Skill: &battle.Skill{Action: battle.ActHeal, Power: 15}},
	}

	allyDef := mustBuild(tables, 411, 60, buildRng, "Ally_Def", 0) // 밤, 탱커형
	allyDef.Defending = true                                       // 방어모드
	allyDef.Resist = 45                                            // 탱커: 상태이상 저항 강화
	allyDef.Active = &battle.Skill{ // 방어호령
		Name: "방어호령", Target: battle.TgtAlly, TType: battle.TTAll,
		Action: battle.ActBuff, Stat: battle.StatDefence, Op: battle.OpPercent, Delta: 25, Dur: 3, MaxCool: 4,
	}
	allyDef.Passives = []*battle.Passive{{Name: "가시갑옷", Event: battle.OnHit, PTarget: battle.POther, Chance: 60,
		Skill: &battle.Skill{Action: battle.ActAttack, Power: 0.7}}}

	allySpd := mustBuild(tables, 1061, 60, buildRng, "Ally_Spd", 0) // 월식, 스피드형
	allySpd.Active = &battle.Skill{ // 정화
		Name: "정화", Target: battle.TgtAlly, TType: battle.TTAll,
		Action: battle.ActCleanse, MaxCool: 3,
	}

	squad := []*battle.Dino{allyAtk, allyDef, allySpd}

	// ── 적 웨이브(side 1): 실제 다이노를 웨이브마다 레벨을 올려 생성 + 일부 스킬 ────
	w1a := mustBuild(tables, 112, 40, buildRng, "W1_112", 1)
	w1b := mustBuild(tables, 212, 42, buildRng, "W1_212", 1)
	w1b.Active = &battle.Skill{ // 맹독
		Name: "맹독", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActCC, CC: battle.CCSpec{Name: "중독", DoT: 7, Duration: 3}, MaxCool: 2,
	}

	w2a := mustBuild(tables, 113, 50, buildRng, "W2_113", 1)
	w2a.Active = &battle.Skill{ // 약화
		Name: "약화", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActDebuff, Stat: battle.StatAttack, Op: battle.OpPercent, Delta: 25, Dur: 2, MaxCool: 3,
	}
	w2b := mustBuild(tables, 213, 50, buildRng, "W2_213", 1)
	w2c := mustBuild(tables, 121, 52, buildRng, "W2_121", 1)

	w3boss := mustBuild(tables, 1061, 58, buildRng, "W3_Boss", 1) // 월식 보스(고레벨)
	w3boss.Active = &battle.Skill{ // 공포
		Name: "공포", Target: battle.TgtEnemy, TType: battle.TTSingle,
		Action: battle.ActCC, CC: battle.CCSpec{Name: "기절", ActLock: true, Duration: 1}, MaxCool: 3,
	}
	w3boss.Passives = []*battle.Passive{{Name: "최후의발악", Event: battle.OnDeath, PTarget: battle.PEnemies,
		Skill: &battle.Skill{Action: battle.ActAttack, Power: 1.2}}}
	w3guard := mustBuild(tables, 221, 50, buildRng, "W3_Guard", 1)

	stage := &battle.Stage{
		Squad: squad,
		Waves: [][]*battle.Dino{
			{w1a, w1b},
			{w2a, w2b, w2c},
			{w3boss, w3guard},
		},
	}

	// 편성표 출력(실제 스탯이 테이블+레벨에서 나온 것임을 확인).
	fmt.Println("======== 편성 (Table/csv 실스탯 · 레벨 성장) ========")
	printRoster := func(title string, ds []*battle.Dino) {
		fmt.Println("--", title)
		for _, d := range ds {
			fmt.Printf("   %-9s Lv%-3d %-3s  HP %-6.0f ATK %-5.0f DEF %-5.0f SPD %-5.0f\n",
				d.Name, d.Level, attrName(d.Attribute), d.MaxHP, d.Attack, d.Defence, d.Aux)
		}
	}
	printRoster("편대(side0)", squad)
	for i, w := range stage.Waves {
		printRoster(fmt.Sprintf("Wave %d(side1)", i+1), w)
	}
	fmt.Println()

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
