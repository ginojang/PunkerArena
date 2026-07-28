// WebSocket 전투 서버: go run ./cmd/server (localhost:8080)
// 웹 클라(React)가 ws://localhost:8080/ws 로 접속. 클라는 CLI/버튼 명령(텍스트)을 보내고,
// 서버는 전투를 한 줄씩 스트리밍(JSON)한다. 플레이어 편대(side 0)의 턴엔 선택을 요청·대기한다.
package main

import (
	"fmt"
	"log"
	"math/rand"
	"net/http"
	"strings"
	"sync"
	"time"

	"github.com/gorilla/websocket"

	"punker/logic-server/battle"
	"punker/logic-server/data"
	"punker/logic-server/scenario"
)

const (
	addr   = "localhost:8080"
	csvDir = "data/csv"
)

var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool { return true }, // 로컬 개발: 오리진 허용
}

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

type dinoJSON struct {
	Name  string  `json:"name"`
	Side  int     `json:"side"`
	HP    float64 `json:"hp"`
	MaxHP float64 `json:"maxHp"`
	Alive bool    `json:"alive"`
	Attr  string  `json:"attr"`
	Atk   float64 `json:"atk"`
	Def   float64 `json:"def"`
	Spd   float64 `json:"spd"`
	Lv    int     `json:"lv"`
	Defn  bool    `json:"defending"`
}

func snapshot(dinos []*battle.Dino) []dinoJSON {
	out := make([]dinoJSON, 0, len(dinos))
	for _, d := range dinos {
		out = append(out, dinoJSON{
			Name: d.Name, Side: d.Side, HP: d.HP, MaxHP: d.MaxHP, Alive: d.Alive(),
			Attr: attrName(d.Attribute), Atk: d.Attack, Def: d.Defence, Spd: d.Aux,
			Lv: d.Level, Defn: d.Defending,
		})
	}
	return out
}

// actionJSON: 플레이어 턴에 제시하는 선택지.
type actionJSON struct {
	Idx         int    `json:"idx"` // a.Actives 인덱스, -1 = 평타
	Name        string `json:"name"`
	NeedsTarget bool   `json:"needsTarget"`
	TargetSide  string `json:"targetSide"` // "enemy" | "ally" | ""
}

type msg struct {
	Type    string       `json:"type"`
	Log     string       `json:"log,omitempty"`
	Turn    int          `json:"turn,omitempty"`
	Dinos   []dinoJSON   `json:"dinos,omitempty"`
	Win     bool         `json:"win,omitempty"`
	Waves   int          `json:"waves,omitempty"`
	Total   int          `json:"total,omitempty"`
	Turns   int          `json:"turns,omitempty"`
	Text    string       `json:"text,omitempty"`
	Actor   string       `json:"actor,omitempty"`   // turn: 행동 다이노
	Actions []actionJSON `json:"actions,omitempty"` // turn: 선택지
}

// choice: 클라가 보낸 턴 선택.
type choice struct {
	idx    int
	auto   bool
	target string
}

func main() {
	tables, err := data.LoadTables(csvDir)
	if err != nil {
		log.Fatalf("테이블 로드 실패: %v (logic-server/ 에서 실행하세요)", err)
	}
	http.HandleFunc("/ws", func(w http.ResponseWriter, r *http.Request) {
		conn, err := upgrader.Upgrade(w, r, nil)
		if err != nil {
			return
		}
		defer conn.Close()
		newSession(conn, tables).run()
	})
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintln(w, "PunkerArena logic-server — connect a client to ws://"+addr+"/ws")
	})
	log.Printf("전투 WS 서버 시작: ws://%s/ws", addr)
	log.Fatal(http.ListenAndServe(addr, nil))
}

// session: 한 연결의 상태.
type session struct {
	conn    *websocket.Conn
	tables  *data.Tables
	writeMu sync.Mutex
	chMu    sync.Mutex
	pending chan choice // 결정 대기 채널(없으면 nil)
	done    chan struct{}
	running bool
	delay   time.Duration
}

func newSession(conn *websocket.Conn, t *data.Tables) *session {
	return &session{conn: conn, tables: t, done: make(chan struct{}), delay: 35 * time.Millisecond}
}

func (s *session) send(m msg) {
	s.writeMu.Lock()
	defer s.writeMu.Unlock()
	_ = s.conn.WriteJSON(m)
}

// run: 명령 읽기 루프.
func (s *session) run() {
	defer close(s.done)
	s.send(msg{Type: "info", Text: "연결됨. start=플레이어 조작 · watch=관전(AI) · speed <ms> · help"})
	for {
		_, raw, err := s.conn.ReadMessage()
		if err != nil {
			return
		}
		f := strings.Fields(strings.TrimSpace(string(raw)))
		if len(f) == 0 {
			continue
		}
		switch f[0] {
		case "start", "s":
			s.begin(false)
		case "watch", "auto":
			s.begin(true)
		case "choose", "c":
			s.feed(f)
		case "reset", "r":
			s.send(msg{Type: "reset"})
		case "speed":
			if len(f) > 1 {
				if v := atoiSafe(f[1]); v > 0 {
					s.delay = time.Duration(v) * time.Millisecond
					s.send(msg{Type: "info", Text: fmt.Sprintf("스트리밍 간격 = %dms", v)})
				}
			}
		case "help", "h", "?":
			s.send(msg{Type: "info", Text: "start=조작, watch=관전, choose <idx> [대상]=선택, speed <ms>, reset"})
		default:
			s.send(msg{Type: "info", Text: "알 수 없는 명령: " + f[0]})
		}
	}
}

// feed: choose 명령을 대기 중인 결정 채널로 전달.
func (s *session) feed(f []string) {
	var c choice
	if len(f) > 1 && f[1] == "auto" {
		c.auto = true
	} else if len(f) > 1 {
		c.idx = atoiSigned(f[1])
		if len(f) > 2 {
			c.target = f[2]
		}
	}
	s.chMu.Lock()
	ch := s.pending
	s.chMu.Unlock()
	if ch != nil {
		select {
		case ch <- c:
		default:
		}
	}
}

// begin: 전투 시작(중복 방지). interactive=false면 관전(AI).
func (s *session) begin(watch bool) {
	if s.running {
		return
	}
	s.running = true
	go func() {
		defer func() { s.running = false }()
		s.runStage(watch)
	}()
}

// runStage: 데모 스테이지를 새로 구성하고 스트리밍. 플레이어 턴엔 decide로 선택 요청.
func (s *session) runStage(watch bool) {
	rand.Seed(1)
	_, _, stage := scenario.Demo(s.tables)

	s.send(msg{Type: "roster", Dinos: snapshot(append(append([]*battle.Dino{}, stage.Squad...), stage.Waves[0]...))})
	stage.OnLog = func(b *battle.Battle, line string) {
		s.send(msg{Type: "tick", Turn: b.Turn, Log: line, Dinos: snapshot(b.Dinos)})
		time.Sleep(s.delay)
	}
	if !watch {
		stage.PlayerSide = 0
		stage.Decide = s.decide
	}
	res := stage.Run(500)
	s.send(msg{Type: "end", Win: res.Win, Waves: res.WavesClear, Total: res.TotalWaves, Turns: res.Turns})
}

// decide: 플레이어 편대 다이노의 한 턴 선택을 클라에 요청하고 응답을 기다린다.
func (s *session) decide(b *battle.Battle, a *battle.Dino) battle.Decision {
	actions := []actionJSON{{Idx: -1, Name: "평타", NeedsTarget: true, TargetSide: "enemy"}}
	for i, sk := range a.Actives {
		if !sk.Ready() {
			continue
		}
		needs, side := targetInfo(sk)
		actions = append(actions, actionJSON{Idx: i, Name: sk.Name, NeedsTarget: needs, TargetSide: side})
	}

	ch := make(chan choice, 1)
	s.chMu.Lock()
	s.pending = ch
	s.chMu.Unlock()

	s.send(msg{Type: "turn", Turn: b.Turn, Actor: a.Name, Actions: actions, Dinos: snapshot(b.Dinos)})

	var c choice
	select {
	case c = <-ch:
	case <-s.done: // 연결 종료 → 남은 턴은 AI로 자동 진행
		return battle.Decision{Auto: true}
	}
	s.chMu.Lock()
	s.pending = nil
	s.chMu.Unlock()

	if c.auto {
		return battle.Decision{Auto: true}
	}
	var skill *battle.Skill
	if c.idx >= 0 && c.idx < len(a.Actives) && a.Actives[c.idx].Ready() {
		skill = a.Actives[c.idx]
	}
	var target *battle.Dino
	if c.target != "" {
		for _, d := range b.Dinos {
			if d.Name == c.target && d.Alive() {
				target = d
				break
			}
		}
	}
	return battle.Decision{Skill: skill, Target: target}
}

// targetInfo: 스킬의 대상 지정 필요 여부/진영.
func targetInfo(sk *battle.Skill) (needs bool, side string) {
	if sk.TType == battle.TTAll {
		return false, ""
	}
	switch sk.Target {
	case battle.TgtSelf:
		return false, ""
	case battle.TgtAlly:
		return true, "ally"
	default:
		return true, "enemy"
	}
}

func atoiSafe(s string) int {
	n := 0
	for _, c := range s {
		if c < '0' || c > '9' {
			return 0
		}
		n = n*10 + int(c-'0')
	}
	return n
}

func atoiSigned(s string) int {
	if strings.HasPrefix(s, "-") {
		return -atoiSafe(s[1:])
	}
	return atoiSafe(s)
}
