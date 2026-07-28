package main

import (
	"fmt"
	"math/rand"
	"strings"

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

// loadSkill: 실제 SkillTBL id를 다이노에 장착. 미지원이면 경고 후 평타 폴백.
func loadSkill(t *data.Tables, d *battle.Dino, skillIdx, skillLv int) {
	if err := t.BuildSkillOn(d, skillIdx, skillLv); err != nil {
		fmt.Printf("   [스킬 폴백] %s ← skill %d: %v\n", d.Name, skillIdx, err)
	}
}

// skillLabel: 다이노에 장착된 스킬 요약(로스터 출력용).
func skillLabel(d *battle.Dino) string {
	var parts []string
	if d.Active != nil {
		parts = append(parts, "액티브 "+d.Active.Name)
	}
	for _, p := range d.Passives {
		parts = append(parts, p.Name)
	}
	if len(parts) == 0 {
		return "평타"
	}
	return strings.Join(parts, ", ")
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

	// ── 플레이어 편대(side 0): 실제 다이노 + 실제 SkillTBL 스킬 장착 ────────────
	const skLv = 3 // 스킬 레벨(SkillLevelTBL)

	allyAtk := mustBuild(tables, 111, 60, buildRng, "Ally_Atk", 0) // 정오, 딜러형
	loadSkill(tables, allyAtk, 1010202, skLv)                     // 액티브: 강타(공격 배율)

	allyDef := mustBuild(tables, 411, 60, buildRng, "Ally_Def", 0) // 밤, 탱커형
	allyDef.Defending = true                                       // 방어모드
	allyDef.Resist = 45                                            // 탱커: 상태이상 저항 강화
	loadSkill(tables, allyDef, 2010204, skLv)                      // 액티브: 방어력 버프(아군 전체)

	allySpd := mustBuild(tables, 1061, 60, buildRng, "Ally_Spd", 0) // 월식, 스피드형
	loadSkill(tables, allySpd, 2010203, skLv)                       // 액티브: 회복

	squad := []*battle.Dino{allyAtk, allyDef, allySpd}

	// ── 적 웨이브(side 1): 실제 다이노 + 일부 실제 스킬(패시브 CC 포함) ──────────
	w1a := mustBuild(tables, 112, 40, buildRng, "W1_112", 1)
	w1b := mustBuild(tables, 212, 42, buildRng, "W1_212", 1)
	loadSkill(tables, w1b, 1020401, skLv) // 패시브: 공격 성사 시 중독(DoT)

	w2a := mustBuild(tables, 113, 50, buildRng, "W2_113", 1)
	loadSkill(tables, w2a, 1010202, skLv) // 액티브: 강타
	w2b := mustBuild(tables, 213, 50, buildRng, "W2_213", 1)
	w2c := mustBuild(tables, 121, 52, buildRng, "W2_121", 1)

	w3boss := mustBuild(tables, 1061, 58, buildRng, "W3_Boss", 1) // 월식 보스(고레벨)
	loadSkill(tables, w3boss, 5020404, skLv)                      // 패시브: 공격 성사 시 빙결(행동불가)
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
			fmt.Printf("   %-9s Lv%-3d %-3s  HP %-6.0f ATK %-5.0f DEF %-5.0f SPD %-5.0f  [명중%.0f 회피%.0f 치명%.0f 관통%.0f 저항%.0f 행운%.0f]  | %s\n",
				d.Name, d.Level, attrName(d.Attribute), d.MaxHP, d.Attack, d.Defence, d.Aux,
				d.HitRate, d.AvoidRate, d.CritRate, d.PenetRate, d.Resist, d.Luck, skillLabel(d))
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
