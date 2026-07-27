package battle

import (
	"fmt"
	"math/rand"
	"sort"
)

// Battle: 한 판의 전투 상태 + 로그
type Battle struct {
	Dinos []*Dino
	Log   []string
	Turn  int
}

func (b *Battle) logf(f string, a ...any) { b.Log = append(b.Log, fmt.Sprintf(f, a...)) }

// 턴 순서: aux(속도) 내림차순, 동률이면 luck (ServerWaveCore WaveStatusActionOrder 기준)
func (b *Battle) order() []*Dino {
	live := make([]*Dino, 0, len(b.Dinos))
	for _, d := range b.Dinos {
		if d.Alive() {
			live = append(live, d)
		}
	}
	sort.SliceStable(live, func(i, j int) bool {
		if live[i].Aux != live[j].Aux {
			return live[i].Aux > live[j].Aux
		}
		return live[i].Luck > live[j].Luck
	})
	return live
}

func (b *Battle) sideAlive(side int) int {
	n := 0
	for _, d := range b.Dinos {
		if d.Side == side && d.Alive() {
			n++
		}
	}
	return n
}

func (b *Battle) randomEnemy(a *Dino) *Dino {
	enemies := make([]*Dino, 0)
	for _, d := range b.Dinos {
		if d.Side != a.Side && d.Alive() {
			enemies = append(enemies, d)
		}
	}
	if len(enemies) == 0 {
		return nil
	}
	return enemies[rand.Intn(len(enemies))]
}

// Run: 완전 자동 전투. 반환 winner side (0 아군 / 1 적 / -1 무승부·시간초과)
func (b *Battle) Run(maxTurns int) int {
	for round := 1; ; round++ {
		b.logf("--- Round %d ---", round)
		for _, a := range b.order() {
			if !a.Alive() {
				continue
			}
			if b.sideAlive(0) == 0 || b.sideAlive(1) == 0 {
				goto done
			}
			b.Turn++
			if b.Turn > maxTurns {
				b.logf("[MAX] 턴 초과(%d)", maxTurns)
				goto done
			}
			t := b.randomEnemy(a)
			if t == nil {
				continue
			}
			r := Attack(a, t)
			if r.Avoided {
				b.logf("[T%d] %s -> %s : 회피!", b.Turn, a.Name, t.Name)
				continue
			}
			t.HP -= r.Damage
			if t.HP < 0 {
				t.HP = 0
			}
			tag := ""
			if r.Crit {
				tag += " CRIT"
			}
			if r.AttrWin == 0 {
				tag += " 속성+"
			}
			b.logf("[T%d] %s -> %s : -%.0f%s (hp %.0f/%.0f)", b.Turn, a.Name, t.Name, r.Damage, tag, t.HP, t.MaxHP)
			if !t.Alive() {
				b.logf("      * %s 사망 (아군 %d / 적 %d)", t.Name, b.sideAlive(0), b.sideAlive(1))
			}
		}
	}
done:
	if b.sideAlive(1) == 0 {
		return 0
	}
	if b.sideAlive(0) == 0 {
		return 1
	}
	return -1
}
