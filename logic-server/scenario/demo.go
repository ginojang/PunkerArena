// Package scenario: 데모 편성(스테이지) 구성을 헤드리스 CLI와 WS 서버가 공유한다.
package scenario

import (
	"math/rand"

	"punker/logic-server/battle"
	"punker/logic-server/data"
)

// Entry: 편성 항목 — 다이노 + 원본 테이블 idx(스킬 파생 설명용).
type Entry struct {
	D   *battle.Dino
	Idx int
}

// Demo: 실 밸런스 테이블로 편대·적 웨이브를 구성해 스테이지를 만든다. 결정적(생성 시드 고정).
func Demo(t *data.Tables) (squad []Entry, waves [][]Entry, stage *battle.Stage) {
	rng := rand.New(rand.NewSource(42))
	const skLv = 3

	spawn := func(idx, level int, name string, side int) Entry {
		d, err := t.BuildDino(idx, level, rng, name, side)
		if err != nil {
			panic(err)
		}
		t.AutoEquipSkills(d, idx, skLv)
		return Entry{D: d, Idx: idx}
	}

	squad = []Entry{
		spawn(111, 60, "Ally_Atk", 0),  // 정오, 딜러형
		spawn(411, 60, "Ally_Def", 0),  // 밤, 탱커형
		spawn(1061, 60, "Ally_Spd", 0), // 월식, 스피드형
	}
	squad[1].D.Defending = true // 탱커 방어모드

	waves = [][]Entry{
		{spawn(112, 40, "W1_112", 1), spawn(212, 42, "W1_212", 1)},
		{spawn(113, 50, "W2_113", 1), spawn(213, 50, "W2_213", 1), spawn(511, 38, "W2_C5", 1)},
		{spawn(1061, 58, "W3_Boss", 1), spawn(221, 50, "W3_Guard", 1)},
	}

	dinos := func(es []Entry) []*battle.Dino {
		out := make([]*battle.Dino, len(es))
		for i, e := range es {
			out[i] = e.D
		}
		return out
	}
	sw := make([][]*battle.Dino, len(waves))
	for i, w := range waves {
		sw[i] = dinos(w)
	}
	stage = &battle.Stage{Squad: dinos(squad), Waves: sw}
	return squad, waves, stage
}
