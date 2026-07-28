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

// unit: 데모 편성 단위 — 다이노 + 원본 테이블 idx(스킬 파생 출력용).
type unit struct {
	d   *battle.Dino
	idx int
}

// skillLabel: 다이노에 장착된 스킬 요약(로스터 출력용).
func skillLabel(d *battle.Dino) string {
	var parts []string
	if d.Active != nil {
		s := "액티브 " + d.Active.Name
		if n := len(d.Active.Riders); n > 0 {
			s += fmt.Sprintf("(+2차 %d)", n)
		}
		parts = append(parts, s)
	}
	for _, p := range d.Passives {
		s := p.Name
		if n := len(p.Skill.Riders); n > 0 {
			s += fmt.Sprintf("(+2차 %d)", n)
		}
		parts = append(parts, s)
	}
	if len(parts) == 0 {
		return "평타"
	}
	return strings.Join(parts, ", ")
}

func main() {
	rand.Seed(1)                             // 전투 판정 RNG(재현용)
	buildRng := rand.New(rand.NewSource(42)) // 다이노 생성 RNG(전투와 분리)

	tables, err := data.LoadTables(csvDir)
	if err != nil {
		fmt.Println("테이블 로드 실패:", err)
		fmt.Println("→ logic-server/ 디렉터리에서 `go run ./cmd/sim` 로 실행하세요.")
		return
	}

	const skLv = 3 // 스킬 레벨(SkillLevelTBL)

	// spawn: 테이블에서 다이노 생성 + 몸통·파츠에서 스킬 자동 파생.
	spawn := func(idx, level int, name string, side int) unit {
		d, err := tables.BuildDino(idx, level, buildRng, name, side)
		if err != nil {
			panic(err)
		}
		tables.AutoEquipSkills(d, idx, skLv)
		return unit{d, idx}
	}

	// ── 플레이어 편대(side 0) ───────────────────────────────────────
	allyAtk := spawn(111, 60, "Ally_Atk", 0)  // 정오, 딜러형
	allyDef := spawn(411, 60, "Ally_Def", 0)  // 밤, 탱커형
	allyDef.d.Defending = true                // 방어모드(데모 설정)
	allySpd := spawn(1061, 60, "Ally_Spd", 0) // 월식, 스피드형
	squad := []unit{allyAtk, allyDef, allySpd}

	// ── 적 웨이브(side 1) ───────────────────────────────────────────
	waves := [][]unit{
		{spawn(112, 40, "W1_112", 1), spawn(212, 42, "W1_212", 1)},
		{spawn(113, 50, "W2_113", 1), spawn(213, 50, "W2_213", 1), spawn(121, 52, "W2_121", 1)},
		{spawn(1061, 58, "W3_Boss", 1), spawn(221, 50, "W3_Guard", 1)},
	}

	dinosOf := func(us []unit) []*battle.Dino {
		out := make([]*battle.Dino, len(us))
		for i, u := range us {
			out[i] = u.d
		}
		return out
	}
	stageWaves := make([][]*battle.Dino, len(waves))
	for i, w := range waves {
		stageWaves[i] = dinosOf(w)
	}
	stage := &battle.Stage{Squad: dinosOf(squad), Waves: stageWaves}

	// 편성표 + 스킬 파생 출력(스탯=테이블+레벨+파츠, 스킬=몸통+파츠 파생임을 확인).
	fmt.Println("======== 편성 (실스탯·레벨성장·파츠 / 스킬=몸통+파츠 자동파생) ========")
	printRoster := func(title string, us []unit) {
		fmt.Println("--", title)
		for _, u := range us {
			d := u.d
			fmt.Printf("   %-9s Lv%-3d %-3s  HP %-6.0f ATK %-5.0f DEF %-5.0f SPD %-5.0f  [명중%.0f 치명%.0f 관통%.0f 저항%.0f 행운%.0f]\n",
				d.Name, d.Level, attrName(d.Attribute), d.MaxHP, d.Attack, d.Defence, d.Aux,
				d.HitRate, d.CritRate, d.PenetRate, d.Resist, d.Luck)
			fmt.Printf("             파생풀[%s] → 장착[%s]\n", tables.DescribeSkills(u.idx), skillLabel(d))
		}
	}
	printRoster("편대(side0)", squad)
	for i, w := range waves {
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
