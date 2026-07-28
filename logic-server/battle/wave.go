package battle

import "fmt"

// Stage: 한 스테이지 = 플레이어 편대(side 0) + 순차 적 웨이브들(side 1).
// [모델] ServerWaveCore: 웨이브 하나는 상태머신이 처리하고, 스테이지는 다음 WaveInfoAPI를
// 순차 공급하는 상위 개념. 편대는 웨이브 간 currentHP를 이월하고 사망은 유지(부활 없음),
// 적은 웨이브마다 신규 그룹.
type Stage struct {
	Squad []*Dino   // side 0 — 웨이브 간 지속(생존 HP 이월, 사망 유지)
	Waves [][]*Dino // 각 원소 = 한 웨이브의 side 1 적 그룹
	Log   []string
	Turn  int // 스테이지 누적 턴
}

// StageResult: 스테이지 전투 결과
type StageResult struct {
	Win        bool
	WavesClear int // 클리어한 웨이브 수
	TotalWaves int
	Turns      int
}

func (s *Stage) logf(f string, a ...any) { s.Log = append(s.Log, fmt.Sprintf(f, a...)) }

// aliveCount: 살아있는 다이노 수
func aliveCount(ds []*Dino) int {
	n := 0
	for _, d := range ds {
		if d.Alive() {
			n++
		}
	}
	return n
}

// Run: 스테이지 전 웨이브를 순차 진행. 편대 전멸 시 패배, 전 웨이브 클리어 시 승리.
// maxTurnsPerWave: 한 웨이브의 턴 상한(무한루프 방지). 초과(무승부)는 미클리어=패배로 처리.
func (s *Stage) Run(maxTurnsPerWave int) StageResult {
	total := len(s.Waves)
	for wi, enemies := range s.Waves {
		if aliveCount(s.Squad) == 0 {
			break
		}
		// 웨이브 전환 시 전투 임시 상태 리셋: 지속 효과 제거 + 스킬 쿨다운 초기화(HP는 이월).
		for _, d := range s.Squad {
			d.clearEffects()
			for _, sk := range d.Actives {
				sk.reset()
			}
			for _, p := range d.Passives {
				p.reset()
			}
		}
		s.logf("========================  Wave %d/%d  ========================", wi+1, total)

		// 이 웨이브용 Battle: 편대(현재 HP 이월) + 신규 적. 같은 포인터라 HP가 자연 이월된다.
		b := &Battle{StageStart: wi == 0} // 첫 웨이브에서만 OnStageStart
		b.Dinos = append(b.Dinos, s.Squad...)
		b.Dinos = append(b.Dinos, enemies...)

		w := b.Run(maxTurnsPerWave)
		s.Log = append(s.Log, b.Log...)
		s.Turn += b.Turn

		if w == 1 { // 적 승 = 편대 전멸 → 스테이지 패배
			s.logf(">>> 편대 전멸 — Wave %d에서 패배", wi+1)
			return StageResult{Win: false, WavesClear: wi, TotalWaves: total, Turns: s.Turn}
		}
		if aliveCount(enemies) > 0 { // 시간초과 등으로 적 잔존 → 미클리어 → 실패
			s.logf(">>> Wave %d 미클리어(적 %d 잔존) — 패배", wi+1, aliveCount(enemies))
			return StageResult{Win: false, WavesClear: wi, TotalWaves: total, Turns: s.Turn}
		}
		s.logf(">>> Wave %d 클리어! (편대 생존 %d/%d)", wi+1, aliveCount(s.Squad), len(s.Squad))
	}
	return StageResult{Win: true, WavesClear: total, TotalWaves: total, Turns: s.Turn}
}
